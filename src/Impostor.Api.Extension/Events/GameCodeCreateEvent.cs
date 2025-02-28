using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Api.Extension.Events;

public class GameCodeCreateEvent(GameCreationEvent? creationEvent = null) : IEvent
{
    public GameCreationEvent? CreationEvent { get; } = creationEvent;
    public EventOutcome<GameCode>? Result { get; set; }
}
