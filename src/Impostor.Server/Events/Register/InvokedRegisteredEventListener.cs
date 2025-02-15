using System;
using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Server.Events.Register;

internal class InvokedRegisteredEventListener : IRegisteredEventListener
{
    private readonly IRegisteredEventListener _innerObject;
    private readonly Func<Func<Task>, Task> _invoker;

    public InvokedRegisteredEventListener(IRegisteredEventListener innerObject, Func<Func<Task>, Task> invoker)
    {
        _innerObject = innerObject;
        _invoker = invoker;
    }

    public Type EventType
    {
        get => _innerObject.EventType;
    }

    public EventPriority Priority
    {
        get => _innerObject.Priority;
    }

    public ValueTask InvokeAsync(object? eventHandler, object @event, IServiceProvider provider)
    {
        return new ValueTask(_invoker(() => _innerObject.InvokeAsync(eventHandler, @event, provider).AsTask()));
    }
}
