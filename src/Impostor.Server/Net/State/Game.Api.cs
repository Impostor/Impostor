using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net;
using Impostor.Api.Net;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        IClientPlayer IGame.Host => Host;

        IGameNet IGame.GameNet => GameNet;
    }
}