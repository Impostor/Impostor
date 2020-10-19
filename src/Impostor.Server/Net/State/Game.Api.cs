using Impostor.Server.Games;

namespace Impostor.Server.Net.State
{
    internal partial class Game : IGame
    {
        IClientPlayer IGame.Host => Host;
    }
}