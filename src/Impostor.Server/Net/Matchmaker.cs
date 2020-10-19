using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Hazel;
using Hazel.Udp;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Hazel
{
    internal class Matchmaker
    {
        private readonly ClientManager _clientManager;
        private readonly ILogger<Matchmaker> _logger;
        private readonly ILogger<Net.Hazel.HazelConnection> _connectionLogger;
        private UdpConnectionListener _connection;

        public Matchmaker(
            ILogger<Matchmaker> logger,
            ClientManager clientManager,
            ILogger<Net.Hazel.HazelConnection> connectionLogger)
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

            var connection = new Net.Hazel.HazelConnection(e.Connection, _connectionLogger);

            // Register client
            await _clientManager.RegisterConnectionAsync(connection, name, clientVersion);
        }
    }
}