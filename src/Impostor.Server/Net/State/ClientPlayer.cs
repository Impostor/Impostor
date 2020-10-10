using System.Threading.Tasks;
using Impostor.Server.Games;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal class ClientPlayer : IClientPlayer
    {
        public ClientPlayer(IClient client, Game game)
        {
            Game = game;
            Client = client;
            Limbo = LimboStates.PreSpawn;
        }

        public IClient Client { get; }

        public Game Game { get; }

        /// <inheritdoc />
        public LimboStates Limbo { get; set; }

        /// <inheritdoc />
        IClient IClientPlayer.Client => Client;

        /// <inheritdoc />
        IGame IClientPlayer.Game => Game;

        /// <inheritdoc />
        public ValueTask KickAsync()
        {
            return Game.HandleKickPlayer(Client.Id, false);
        }

        /// <inheritdoc />
        public ValueTask BanAsync()
        {
            return Game.HandleKickPlayer(Client.Id, true);
        }
    }
}