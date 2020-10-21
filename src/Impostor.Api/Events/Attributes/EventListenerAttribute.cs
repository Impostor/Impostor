using System;

namespace Impostor.Api.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventListenerAttribute : Attribute
    {
        public EventListenerAttribute(EventPriority priority = EventPriority.Normal)
        {
            Priority = priority;
        }

        public EventListenerAttribute(Type @event, EventPriority priority = EventPriority.Normal)
        {
            Priority = priority;
            Event = @event;
        }

        /// <summary>
        ///     The priority of the event listener.
        /// </summary>
        public EventPriority Priority { get; set; }

        /// <summary>
        ///     The events that the listener is listening to.
        /// </summary>
        public Type? Event { get; set; }

        /// <summary>
        ///     If set to true, the listener will be called regardless of the <see cref="IEventCancelable.IsCancelled"/>.
        /// </summary>
        public bool IgnoreCancelled { get; set; }
    }
}