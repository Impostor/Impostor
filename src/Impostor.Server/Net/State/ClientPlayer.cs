using System.Threading.Tasks;

namespace Impostor.Server.Net.State
{
    internal partial class ClientPlayer : IClientPlayer
    {
        public ClientPlayer(ClientBase client, Game game)
        {
            Game = game;
            Client = client;
            Limbo = LimboStates.PreSpawn;
        }

        public ClientBase Client { get; }

        public Game Game { get; }

        /// <inheritdoc />
        public LimboStates Limbo { get; set; }

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