using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;

namespace Impostor.Server.Events
{
    public class GameEndedEvent : IGameEndedEvent
    {
        public GameEndedEvent(IGame game, GameOverReason gameOverReason)
        {
            Game = game;
            GameOverReason = gameOverReason;
        }

        public IGame Game { get; }

        public GameOverReason GameOverReason { get; }
    }
}
