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
        ///     Gets or sets the priority of the event listener.
        /// </summary>
        public EventPriority Priority { get; set; }

        /// <summary>
        ///     Gets or sets the event that the listener is listening to.
        /// </summary>
        public Type? Event { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the listener will be called regardless of the <see cref="IEventCancelable.IsCancelled" />.
        /// </summary>
        public bool IgnoreCancelled { get; set; }
    }
}
