using System.Threading.Tasks;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal partial class ClientPlayer : IClientPlayer
    {
        private readonly GameManager _gameManager;

        public ClientPlayer(Client client, GameManager gameManager)
        {
            _gameManager = gameManager;
            
            Client = client;
            Limbo = LimboStates.PreSpawn;
        }
        
        public Client Client { get; }
        public Game Game { get; set; }
        public LimboStates Limbo { get; set; }

        public async ValueTask SendDisconnectReason(DisconnectReason reason, string message = null)
        {
            using (var packet = Client.Connection.CreateMessage(MessageType.Reliable))
            {
                Message01JoinGame.SerializeError(packet, false, reason, message);
                await packet.SendAsync();
            }
        }

        IClient IClientPlayer.Client => Client;

        IGame IClientPlayer.Game => Game;
    }
}