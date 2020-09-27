using System;
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
    internal class Matchmaker
    {
        private readonly ILogger<Matchmaker> _logger;
        private readonly ServerConfig _serverConfig;
        private readonly IClientManager _clientManager;
        private readonly UdpConnectionListener _connection;
        
        public Matchmaker(
            ILogger<Matchmaker> logger, 
            IOptions<ServerConfig> serverConfig, 
            IClientManager clientManager)
        {
            _logger = logger;
            _serverConfig = serverConfig.Value;
            _clientManager = clientManager;
            _connection = new UdpConnectionListener(new IPEndPoint(IPAddress.Parse(_serverConfig.ListenIp), _serverConfig.ListenPort), IPMode.IPv4, s =>
            {
                _logger.LogWarning("Log from Hazel: {0}", s);
            });
            
            _connection.NewConnection += OnNewConnection;
        }
        
        public IPEndPoint EndPoint => _connection.EndPoint;

        private void OnNewConnection(NewConnectionEventArgs e)
        {
            try
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
            
                // Create client.
                _clientManager.Create(clientName, e.Connection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in new connection.");
            }
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