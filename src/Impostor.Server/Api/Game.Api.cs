using Impostor.Server.Games;

// ReSharper disable once CheckNamespace
namespace Impostor.Server.Net.State
{
    internal partial class Game : IGame
    {
        IClientPlayer IGame.Host => Host;
    }
}