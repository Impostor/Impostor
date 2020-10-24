using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        IClientPlayer IGame.Host => Host;

        IGameNet IGame.GameNet => GameNet;
    }
}