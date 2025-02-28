using System.Diagnostics.CodeAnalysis;
using Impostor.Api.Events;
using Impostor.Api.Extension.Events;
using Impostor.Api.Games;

namespace GameCodePlugin;

#pragma warning disable CA1822
public class GameCodeEventListener : IEventListener
{
    [EventListener]

    public void OnCreateCode(GameCodeCreateEvent @event)
    {
        var state = GameCodePlugin.Codes.FirstOrDefault(used => !used);
        if (state == null) return;
        @event.Result = new EventOutcome<GameCode>(state.Code);
        state.Used = true;
    }

    [EventListener]
    public void OnReleaseCode(IGameDestroyedEvent @event)
    {
        var code = @event.Game.Code;
        var state = GameCodePlugin.Codes.FirstOrDefault(state => state.Code == code);
        if (state == null) return;
        state.Used = false;
    }
}
