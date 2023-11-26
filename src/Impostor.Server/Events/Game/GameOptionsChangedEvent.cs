using Impostor.Api.Events;
using Impostor.Api.Games;
using static Impostor.Api.Events.IGameOptionsChangedEvent;

namespace Impostor.Server.Events
{
    public class GameOptionsChangedEvent : IGameOptionsChangedEvent
    {
        public GameOptionsChangedEvent(IGame game, ChangeReason changedBy)
        {
            Game = game;
            ChangedBy = changedBy;
        }

        public ChangeReason ChangedBy { get; }

        public IGame Game { get; }
    }
}
