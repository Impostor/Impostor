using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Net;

namespace Impostor.Server.Events
{
    public class GameCreatedEvent : IGameCreatedEvent
    {
        public GameCreatedEvent(IGame game, IClient? host)
        {
            Game = game;
            Host = host;
        }

        public IGame Game { get; }

        public IClient? Host { get; }
    }
}
