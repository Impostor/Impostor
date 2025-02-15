using System;
using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Server.Events.Register;

internal class InvokedRegisteredEventListener(IRegisteredEventListener innerObject, Func<Func<Task>, Task> invoker)
    : IRegisteredEventListener
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
        return new ValueTask(invoker(() => innerObject.InvokeAsync(eventHandler, @event, provider).AsTask()));
    }
}
