using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.C2S;
using Impostor.Hazel;
using Impostor.Hazel.Extensions;
using Impostor.Server;
using Impostor.Server.Events;
using Impostor.Server.Net;
using Impostor.Server.Net.Factories;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Redirector;
using Impostor.Server.Recorder;
using Impostor.Server.Utils;
using Impostor.Tools.ServerReplay.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Tools.ServerReplay
{
    internal static class Program
    {
        private static readonly ILogger Logger = Log.ForContext(typeof(Program));
        private static readonly Dictionary<int, IHazelConnection> Connections = new Dictionary<int, IHazelConnection>();
        private static readonly Dictionary<int, GameOptionsData> GameOptions = new Dictionary<int, GameOptionsData>();

        private static ServiceProvider _serviceProvider;

        private static ObjectPool<MessageReader> _readerPool;
        private static MockGameCodeFactory _gameCodeFactory;
        private static ClientManager _clientManager;
        private static GameManager _gameManager;
        private static FakeDateTimeProvider _fakeDateTimeProvider;

        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                // .MinimumLevel.Verbose()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var stopwatch = Stopwatch.StartNew();

            foreach (var file in Directory.GetFiles(args[0], "*.dat"))
            {
                // Clear.
                Connections.Clear();
                GameOptions.Clear();

                // Create service provider.
                _serviceProvider = BuildServices();

                // Create required instances.
                _readerPool = _serviceProvider.GetRequiredService<ObjectPool<MessageReader>>();
                _gameCodeFactory = _serviceProvider.GetRequiredService<MockGameCodeFactory>();
                _clientManager = _serviceProvider.GetRequiredService<ClientManager>();
                _gameManager = _serviceProvider.GetRequiredService<GameManager>();
                _fakeDateTimeProvider = _serviceProvider.GetRequiredService<FakeDateTimeProvider>();

                await using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var reader = new BinaryReader(stream))
                {
                    await ParseSession(reader);
                }
            }

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            Logger.Information($"Took {elapsedMilliseconds}ms.");
        }

        private static ServiceProvider BuildServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton(new ServerEnvironment
            {
                IsReplay = true,
            });

            services.AddSingleton<FakeDateTimeProvider>();
            services.AddSingleton<IDateTimeProvider>(p => p.GetRequiredService<FakeDateTimeProvider>());

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });

            services.AddSingleton<GameManager>();
            services.AddSingleton<IGameManager>(p => p.GetRequiredService<GameManager>());

            services.AddSingleton<MockGameCodeFactory>();
            services.AddSingleton<IGameCodeFactory>(p => p.GetRequiredService<MockGameCodeFactory>());

            services.AddSingleton<ClientManager>();
            services.AddSingleton<IClientFactory, ClientFactory<Client>>();
            services.AddSingleton<INodeLocator, NodeLocatorNoOp>();
            services.AddSingleton<IEventManager, EventManager>();

            services.AddEventPools();
            services.AddHazel();

            return services.BuildServiceProvider();
        }

        private static async Task ParseSession(BinaryReader reader)
        {
            var protocolVersion = (ServerReplayVersion)reader.ReadUInt32();
            if (protocolVersion < ServerReplayVersion.Initial || protocolVersion > ServerReplayVersion.Latest)
            {
                throw new NotSupportedException("Session's protocol version is unsupported");
            }

            var startTime = _fakeDateTimeProvider.UtcNow = DateTimeOffset.FromUnixTimeMilliseconds(reader.ReadInt64());
            var serverVersion = reader.ReadString();

            Logger.Information("Loaded session (server: {ServerVersion}, recorded at {StartTime})", serverVersion, startTime);

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var dataLength = reader.ReadInt32();
                var data = reader.ReadBytes(dataLength - 4);

                await using (var stream = new MemoryStream(data))
                using (var readerInner = new BinaryReader(stream))
                {
                    _fakeDateTimeProvider.UtcNow = startTime + TimeSpan.FromMilliseconds(readerInner.ReadUInt32());
                    await ParsePacket(readerInner);
                }
            }
        }

        private static async Task ParsePacket(BinaryReader reader)
        {
            var dataType = (RecordedPacketType)reader.ReadByte();

            // Read client id.
            var clientId = reader.ReadInt32();

            switch (dataType)
            {
                case RecordedPacketType.Connect:
                    // Read data.
                    var addressLength = reader.ReadByte();
                    var addressBytes = reader.ReadBytes(addressLength);
                    var addressPort = reader.ReadUInt16();
                    var address = new IPEndPoint(new IPAddress(addressBytes), addressPort);
                    var name = reader.ReadString();
                    var gameVersion = reader.ReadInt32();

                    // Create and register connection.
                    var connection = new MockHazelConnection(address);

                    await _clientManager.RegisterConnectionAsync(connection, name, gameVersion);

                    // Store reference for ourselfs.
                    Connections.Add(clientId, connection);
                    break;

                case RecordedPacketType.Disconnect:
                    string reason = null;

                    if (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        reason = reader.ReadString();
                    }

                    await Connections[clientId].Client!.HandleDisconnectAsync(reason);
                    Connections.Remove(clientId);
                    break;

                case RecordedPacketType.Message:
                {
                    var messageType = (MessageType)reader.ReadByte();
                    var tag = reader.ReadByte();
                    var length = reader.ReadInt32();
                    var buffer = reader.ReadBytes(length);
                    using var message = _readerPool.Get();

                    message.Update(buffer, tag: tag);

                    if (tag == MessageFlags.HostGame)
                    {
                        GameOptions.Add(clientId, Message00HostGameC2S.Deserialize(message, out _));
                    }
                    else if (Connections.TryGetValue(clientId, out var client))
                    {
                        await client.Client!.HandleMessageAsync(message, messageType);
                    }

                    break;
                }

                case RecordedPacketType.GameCreated:
                    _gameCodeFactory.Result = GameCode.From(reader.ReadString());

                    await _gameManager.CreateAsync(GameOptions[clientId]);

                    GameOptions.Remove(clientId);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
