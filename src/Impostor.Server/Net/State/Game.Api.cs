using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Net.State
{
    internal partial class Game
    {
        IClientPlayer IGame.Host => Host;
    }
}