using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events
{
    public class GameCreatedEvent : IGameCreatedEvent
    {
        public GameCreatedEvent(IGame game)
        {
            Game = game;
        }

        public IGame Game { get; }
    }
}
