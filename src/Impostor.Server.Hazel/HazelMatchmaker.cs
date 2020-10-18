using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Hazel;
using Hazel.Udp;
using Impostor.Server.Games;
using Impostor.Server.Hazel.Messages;
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

        public async ValueTask StartAsync(IPEndPoint ipEndPoint)
        {
            var mode = ipEndPoint.AddressFamily switch
            {
                AddressFamily.InterNetwork => IPMode.IPv4,
                AddressFamily.InterNetworkV6 => IPMode.IPv6,
                _ => throw new InvalidOperationException()
            };

            _connection = new UdpConnectionListener(ipEndPoint, mode);
            _connection.NewConnection = OnNewConnection;

            await _connection.StartAsync();
        }

        public async ValueTask StopAsync()
        {
            await _connection.DisposeAsync();
        }

        private async ValueTask OnNewConnection(NewConnectionEventArgs e)
        {
            // Handshake.
            var clientVersion = e.HandshakeData.ReadInt32();
            var name = e.HandshakeData.ReadString();

            var connection = new HazelConnection(e.Connection, _connectionLogger);

            // Register client
            await _clientManager.RegisterConnectionAsync(connection, name, clientVersion);
        }

        public IGameMessageWriter CreateGameMessageWriter(IGame game, MessageType messageType)
        {
            return new HazelGameMessageWriter(messageType, game);
        }
    }
}