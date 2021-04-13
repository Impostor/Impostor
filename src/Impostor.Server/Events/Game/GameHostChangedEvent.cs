using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events
{
    public class GameHostChangedEvent : IGameHostChangedEvent
    {
        public GameHostChangedEvent(IGame game, IClientPlayer previousHost, IClientPlayer? newHost)
        {
            Game = game;
            PreviousHost = previousHost;
            NewHost = newHost;
        }

        public IGame Game { get; }

        public IClientPlayer PreviousHost { get; }

        public IClientPlayer? NewHost { get; }
    }
}
