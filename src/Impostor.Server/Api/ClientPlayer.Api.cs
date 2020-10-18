using Impostor.Server.Games;

// ReSharper disable once CheckNamespace
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