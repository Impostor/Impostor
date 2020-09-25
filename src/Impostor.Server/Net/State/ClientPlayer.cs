using Hazel;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    public class ClientPlayer
    {
        public ClientPlayer(Client client)
        {
            Client = client;
        }
        
        public Client Client { get; }
        public Game Game { get; set; }
        public LimboStates LimboState { get; set; }

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