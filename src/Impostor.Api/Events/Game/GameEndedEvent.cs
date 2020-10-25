using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    public class GameEndedEvent : IGameEvent
    {
        public GameEndedEvent(IGame game)
        {
            Game = game;
        }

        public IGame Game { get; }
    }
}