using Impostor.Api.Events;
using Impostor.Api.Games;
using static Impostor.Api.Events.IGameOptionsChangedEvent;

namespace Impostor.Server.Events;

public class GameOptionsChangedEvent(IGame game, ChangeReason changedBy) : IGameOptionsChangedEvent
{
    public ChangeReason ChangedBy { get; } = changedBy;

    public IGame Game { get; } = game;
}
