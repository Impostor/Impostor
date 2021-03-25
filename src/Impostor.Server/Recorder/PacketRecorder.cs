using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
using Impostor.Server.Config;
using Impostor.Server.Net;
using Impostor.Server.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Recorder
{
    /// <summary>
    ///     Records all packets received in <see cref="ClientRecorder.HandleMessageAsync" />.
    /// </summary>
    internal class PacketRecorder : BackgroundService
    {
        private readonly string _path;
        private readonly ILogger<PacketRecorder> _logger;
        private readonly ObjectPool<PacketSerializationContext> _pool;
        private readonly Channel<byte[]> _channel;
        private DateTimeOffset _startTime;

        public PacketRecorder(ILogger<PacketRecorder> logger, IOptions<DebugConfig> options, ObjectPool<PacketSerializationContext> pool)
        {
            var name = $"session_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.dat";

            _path = Path.Combine(options.Value.GameRecorderPath!, name);
            _logger = logger;
            _pool = pool;

            _channel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = false,
            });
        }

        public async Task WriteConnectAsync(ClientRecorder client)
        {
            _logger.LogTrace("Writing Connect.");

            var context = _pool.Get();

            try
            {
                WritePacketHeader(context, RecordedPacketType.Connect);
                WriteClient(context, client, true);
                WriteLength(context);

                await WriteAsync(context.Stream!);
            }
            finally
            {
                _pool.Return(context);
            }
        }

        public async Task WriteDisconnectAsync(ClientRecorder client, string reason)
        {
            _logger.LogTrace("Writing Disconnect.");

            var context = _pool.Get();

            try
            {
                WritePacketHeader(context, RecordedPacketType.Disconnect);
                WriteClient(context, client, false);
                context.Writer.Write(reason);
                WriteLength(context);

                await WriteAsync(context.Stream!);
            }
            finally
            {
                _pool.Return(context);
            }
        }

        public async Task WriteMessageAsync(ClientRecorder client, IMessageReader reader, MessageType messageType)
        {
            _logger.LogTrace("Writing Message.");

            var context = _pool.Get();

            try
            {
                WritePacketHeader(context, RecordedPacketType.Message);
                WriteClient(context, client, false);
                WritePacket(context, reader, messageType);
                WriteLength(context);

                await WriteAsync(context.Stream!);
            }
            finally
            {
                _pool.Return(context);
            }
        }

        public async Task WriteGameCreatedAsync(ClientRecorder client, GameCode gameCode)
        {
            _logger.LogTrace("Writing GameCreated {0}.", gameCode);

            var context = _pool.Get();

            try
            {
                WritePacketHeader(context, RecordedPacketType.GameCreated);
                WriteClient(context, client, false);
                WriteGameCode(context, gameCode);
                WriteLength(context);

                await WriteAsync(context.Stream!);
            }
            finally
            {
                _pool.Return(context);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _startTime = DateTimeOffset.UtcNow;
            _logger.LogInformation("PacketRecorder is enabled, writing packets to {0}.", _path);

            var writer = File.Open(_path, FileMode.CreateNew, FileAccess.Write, FileShare.Read);

            await WriteFileHeaderAsync();

            // Handle messages.
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = await _channel.Reader.ReadAsync(stoppingToken);

                    await writer.WriteAsync(result, stoppingToken);
                    await writer.FlushAsync(stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
            }

            // Clean up.
            await writer.DisposeAsync();
        }

        private async Task WriteFileHeaderAsync()
        {
            var context = _pool.Get();

            try
            {
                context.Writer.Write((uint)ServerReplayVersion.Initial);
                context.Writer.Write(_startTime.ToUnixTimeMilliseconds());
                context.Writer.Write(DotnetUtils.Version);

                await WriteAsync(context.Stream!);
            }
            finally
            {
                _pool.Return(context);
            }
        }

        private void WritePacketHeader(PacketSerializationContext context, RecordedPacketType type)
        {
            // Length placeholder.
            context.Writer.Write(0);

            // Timestamp relative to recording start time.
            context.Writer.Write((uint)(DateTimeOffset.UtcNow - _startTime).TotalMilliseconds);

            context.Writer.Write((byte)type);
        }

        private void WriteClient(PacketSerializationContext context, ClientBase client, bool full)
        {
            var address = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            var addressBytes = address.Address.GetAddressBytes();

            context.Writer.Write(client.Id);

            if (full)
            {
                context.Writer.Write((byte)addressBytes.Length);
                context.Writer.Write(addressBytes);
                context.Writer.Write((ushort)address.Port);
                context.Writer.Write(client.Name);
                context.Writer.Write(client.GameVersion);
            }
        }

        private void WritePacket(PacketSerializationContext context, IMessageReader reader, MessageType messageType)
        {
            context.Writer.Write((byte)messageType);
            context.Writer.Write((byte)reader.Tag);
            context.Writer.Write((int)reader.Length);
            context.Writer.Write(reader.Buffer, reader.Offset, reader.Length);
        }

        private void WriteGameCode(PacketSerializationContext context, in GameCode gameCode)
        {
            context.Writer.Write(gameCode.Code);
        }

        private void WriteLength(PacketSerializationContext context)
        {
            var length = context.Stream.Position;

            context.Stream.Position = 0;
            context.Writer.Write((int)length);
            context.Stream.Position = length;
        }

        private async Task WriteAsync(MemoryStream data)
        {
            await _channel.Writer.WriteAsync(data.ToArray());
        }
    }
}
