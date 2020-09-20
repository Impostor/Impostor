using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net
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
    }
}