using System.Net;
using Hazel;
using Hazel.Udp;
using Impostor.Server.Data;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net
{
    public class Matchmaker
    {
        private readonly ILogger<Matchmaker> _logger;
        private readonly ServerConfig _config;
        private readonly GameManager _gameManager;
        private readonly ClientManager _clientManager;
        private readonly UdpConnectionListener _connection;
        
        public Matchmaker(ILogger<Matchmaker> logger, IOptions<ServerConfig> configOptions, GameManager gameManager)
        {
            _logger = logger;
            _config = configOptions.Value;
            _gameManager = gameManager;
            _clientManager = new ClientManager();
            _connection = new UdpConnectionListener(new IPEndPoint(IPAddress.Parse(_config.ListenIp), _config.ListenPort), IPMode.IPv4, s =>
            {
                _logger.LogWarning("Log from Hazel: {0}", s);
            });
            
            _connection.NewConnection += OnNewConnection;
        }
        
        public IPEndPoint EndPoint => _connection.EndPoint;

        private void OnNewConnection(NewConnectionEventArgs e)
        {
            // Handshake.
            var clientVersion = e.HandshakeData.ReadInt32();
            var clientName = e.HandshakeData.ReadString();

            e.HandshakeData.Recycle();
                
            if (clientVersion != 50516550)
            {
                using (var packet = MessageWriter.Get(SendOption.Reliable))
                {
                    Message01JoinGame.SerializeError(packet, false, DisconnectReason.IncorrectVersion);
                    e.Connection.Send(packet);
                }
                return;
            }
            
            // Register client.
            _clientManager.Add(new Client(_clientManager, _gameManager, _clientManager.NextId(), clientName, e.Connection));
        }

        public void Start()
        {
            _connection.Start();
        }

        public void Stop()
        {
            _connection.Dispose();
        }
    }
}