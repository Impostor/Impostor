using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Net.State
{
    internal partial class Game : IGame
    {
        IClientPlayer IGame.Host => Host;
    }
}