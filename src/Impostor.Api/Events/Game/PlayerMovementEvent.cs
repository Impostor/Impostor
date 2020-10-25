using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    public class PlayerMovementEvent : IGameEvent
    {
        public PlayerMovementEvent(IGame game)
        {
            Game = game;
        }

        public IGame Game { get; }
    }
}