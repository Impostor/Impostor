using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Hazel;
using Hazel.Udp;
using Impostor.Server.Games;
using Impostor.Server.Net;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Hazel
{
    internal class HazelMatchmaker : IMatchmaker
    {
        private readonly IClientManager _clientManager;
        private readonly ILogger<HazelMatchmaker> _logger;
        private readonly ILogger<HazelConnection> _connectionLogger;
        private UdpConnectionListener _connection;

        public HazelMatchmaker(
            ILogger<HazelMatchmaker> logger,
            IClientManager clientManager,
            ILogger<HazelConnection> connectionLogger)
        {
            _logger = logger;
            _clientManager = clientManager;
            _connectionLogger = connectionLogger;
        }

        public ValueTask StartAsync(IPEndPoint ipEndPoint)
        {
            var mode = ipEndPoint.AddressFamily switch
            {
                AddressFamily.InterNetwork => IPMode.IPv4,
                AddressFamily.InterNetworkV6 => IPMode.IPv6,
                _ => throw new InvalidOperationException()
            };

            _connection = new UdpConnectionListener(ipEndPoint, mode, s =>
            {
                _logger.LogWarning("Log from Hazel: {0}", s);
            });

            _connection.NewConnection += OnNewConnection;

            _connection.Start();

            return default;
        }

        public ValueTask StopAsync()
        {
            _connection.Dispose();

            return default;
        }

        private void OnNewConnection(NewConnectionEventArgs e)
        {
            Task.Run(() => HandleNewConnection(e));
        }

        private async Task HandleNewConnection(NewConnectionEventArgs e)
        {
            try
            {
                // Handshake.
                var clientVersion = e.HandshakeData.ReadInt32();
                var name = e.HandshakeData.ReadString();

                e.HandshakeData.Recycle();

                var connection = new HazelConnection(e.Connection, _connectionLogger);

                // Register client
                await _clientManager.RegisterConnectionAsync(connection, name, clientVersion);
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Error in new connection.");
            }
        }

        public IGameMessageWriter CreateGameMessageWriter(IGame game, MessageType messageType)
        {
            return new HazelGameMessageWriter(messageType, game);
        }
    }
}