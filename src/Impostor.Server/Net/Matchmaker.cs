using System.Net;
using Hazel;
using Hazel.Udp;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Impostor.Server.Net
{
    public class Matchmaker
    {
        private static readonly ILogger Logger = Log.ForContext<Matchmaker>();

        private readonly GameManager _gameManager;
        private readonly ClientManager _clientManager;
        private readonly UdpConnectionListener _connection;
        
        public Matchmaker(IPAddress ip, int port)
        {
            _gameManager = new GameManager();
            _clientManager = new ClientManager();
            _connection = new UdpConnectionListener(new IPEndPoint(ip, port), IPMode.IPv4, s =>
            {
                Logger.Warning("Log from Hazel: {0}", s);
            });
            
            _connection.NewConnection += OnNewConnection;
        }

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