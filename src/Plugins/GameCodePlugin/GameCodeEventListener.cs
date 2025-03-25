using System.Diagnostics.CodeAnalysis;
using Impostor.Api.Events;
using Impostor.Api.Extension.Events;
using Impostor.Api.Games;

namespace GameCodePlugin;

#pragma warning disable CA1822
public class GameCodeEventListener(GameCodeStateManager stateManager) : IEventListener
{
    [EventListener]

    public void OnCreateCode(GameCodeCreateEvent @event)
    {
        var code = stateManager.GetCode();
        if (code == null) return;
        @event.Result = new EventOutcome<GameCode>(code.Value);
    }

    [EventListener]
    public void OnReleaseCode(IGameDestroyedEvent @event)
    {
        stateManager.ReleaseCode(@event.Game.Code);
    }
}
