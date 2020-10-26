using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events
{
    public class GameStartingEvent : IGameStartingEvent
    {
        public GameStartingEvent(IGame game)
        {
            Game = game;
        }

        public IGame Game { get; }
    }
}
