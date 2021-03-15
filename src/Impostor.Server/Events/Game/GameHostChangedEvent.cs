using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events
{
    public class GameHostChangedEvent : IGameHostChangedEvent
    {
        public GameHostChangedEvent(IGame game, IClientPlayer host, IClientPlayer oldHost)
        {
            Game = game;
            Host = host;
            OldHost = oldHost;
        }

        public IGame Game { get; }

        public IClientPlayer Host { get; }

        public IClientPlayer OldHost { get; }
    }
}
