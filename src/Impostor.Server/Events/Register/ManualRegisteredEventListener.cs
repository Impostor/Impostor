using System;
using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Server.Events.Register
{
    internal class ManualRegisteredEventListener : IRegisteredEventListener
    {
        public Type EventType { get; } = typeof(object);

        private readonly IManualEventListener _manualEventListener;

        public ManualRegisteredEventListener(IManualEventListener manualEventListener)
        {
            _manualEventListener = manualEventListener;
        }

        public EventPriority Priority => _manualEventListener.Priority;

        public ValueTask InvokeAsync(object eventHandler, object @event, IServiceProvider provider)
        {
            if (@event is IEvent typedEvent)
            {
                return _manualEventListener.Execute(typedEvent);
            }

            return ValueTask.CompletedTask;
        }
    }
}
