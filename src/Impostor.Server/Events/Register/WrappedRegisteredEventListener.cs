using System;
using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Server.Events.Register;

internal class WrappedRegisteredEventListener(IRegisteredEventListener innerObject, object o) : IRegisteredEventListener
{
    public Type EventType
    {
        get => innerObject.EventType;
    }

    public EventPriority Priority
    {
        get => innerObject.Priority;
    }

    public ValueTask InvokeAsync(object? eventHandler, object @event, IServiceProvider provider)
    {
        return innerObject.InvokeAsync(o, @event, provider);
    }
}
