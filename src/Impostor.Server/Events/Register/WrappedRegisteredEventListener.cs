using System;
using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Server.Events.Register
{
    internal class WrappedRegisteredEventListener : IRegisteredEventListener
    {
        private readonly IRegisteredEventListener _innerObject;
        private readonly object _object;

        public WrappedRegisteredEventListener(IRegisteredEventListener innerObject, object o)
        {
            _innerObject = innerObject;
            _object = o;
        }

        public Type EventType => _innerObject.EventType;

        public EventPriority Priority => _innerObject.Priority;

        public ValueTask InvokeAsync(object? eventHandler, object @event, IServiceProvider provider)
        {
            return _innerObject.InvokeAsync(_object, @event, provider);
        }
    }
}
