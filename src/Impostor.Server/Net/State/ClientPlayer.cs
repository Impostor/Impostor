using Hazel;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal partial class ClientPlayer
    {
        private readonly GameManager _gameManager;

        public ClientPlayer(Client client, GameManager gameManager)
        {
            _gameManager = gameManager;
            
            Client = client;
        }
        
        public Client Client { get; }
        public Game Game { get; set; }

        public void SendDisconnectReason(DisconnectReason reason, string message = null)
        {
            using (var packet = MessageWriter.Get(SendOption.Reliable))
            {
                Message01JoinGame.SerializeError(packet, false, reason, message);
                Client.Connection.Send(packet);
            }
        }
    }
}