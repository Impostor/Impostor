using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    public class GameAlterEvent : IGameEvent
    {
        public GameAlterEvent(IGame game, bool isPublic)
        {
            Game = game;
            IsPublic = isPublic;
        }

        public IGame Game { get; }
        public bool IsPublic { get; }
    }
}
