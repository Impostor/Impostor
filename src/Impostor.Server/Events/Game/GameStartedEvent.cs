using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events
{
    public class GameStartedEvent : IGameStartedEvent
    {
        public GameStartedEvent(IGame game)
        {
            Game = game;
        }

        public IGame Game { get; }
    }
}
