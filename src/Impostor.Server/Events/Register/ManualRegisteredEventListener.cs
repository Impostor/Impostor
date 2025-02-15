using System;
using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Server.Events.Register;

internal class ManualRegisteredEventListener(IManualEventListener manualEventListener) : IRegisteredEventListener
{
    public Type EventType { get; } = typeof(object);

    public EventPriority Priority
    {
        get => manualEventListener.Priority;
    }

    public ValueTask InvokeAsync(object? eventHandler, object @event, IServiceProvider provider)
    {
        if (@event is IEvent typedEvent)
        {
            return manualEventListener.Execute(typedEvent);
        }

        return ValueTask.CompletedTask;
    }
}
