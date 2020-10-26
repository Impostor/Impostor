using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events
{
    public class GameAlterEvent : IGameAlterEvent
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
