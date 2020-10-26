using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events
{
    public class GameDestroyedEvent : IGameDestroyedEvent
    {
        public GameDestroyedEvent(IGame game)
        {
            Game = game;
        }

        public IGame Game { get; }
    }
}
