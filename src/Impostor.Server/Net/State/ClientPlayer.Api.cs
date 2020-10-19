using Impostor.Api.Games;
using Impostor.Api.Net;

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