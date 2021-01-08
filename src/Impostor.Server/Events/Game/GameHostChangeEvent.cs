using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events
{
    public class GameHostChangeEvent : IGameHostChangeEvent
    {
        public GameHostChangeEvent(IGame game, IClientPlayer host)
        {
            Game = game;
            Host = host;
        }

        public IGame Game { get; }

        public IClientPlayer Host { get; }
    }
}
