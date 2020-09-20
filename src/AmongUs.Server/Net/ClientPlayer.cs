using AmongUs.Shared.Innersloth.Data;

namespace AmongUs.Server.Net
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