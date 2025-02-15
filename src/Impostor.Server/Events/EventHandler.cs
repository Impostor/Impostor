using Impostor.Api.Events;
using Impostor.Server.Events.Register;

namespace Impostor.Server.Events;

internal readonly struct EventHandler(IEventListener? o, IRegisteredEventListener listener)
{
    public IEventListener? Object { get; } = o;

    public IRegisteredEventListener Listener { get; } = listener;

    public void Deconstruct(out IEventListener? o, out IRegisteredEventListener listener)
    {
        o = Object;
        listener = Listener;
    }
}
