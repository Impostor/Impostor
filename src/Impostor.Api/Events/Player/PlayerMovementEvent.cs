using Impostor.Api.Games;

namespace Impostor.Api.Events.Player
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