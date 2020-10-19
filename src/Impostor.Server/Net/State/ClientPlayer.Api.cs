using Impostor.Server.Games;

namespace Impostor.Server.Net.State
{
    internal partial class ClientPlayer
    {
        /// <inheritdoc />
        IClient IClientPlayer.Client => Client;

        /// <inheritdoc />
        IGame IClientPlayer.Game => Game;
    }
}