using System;

namespace Impostor.Server.Events
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventListenerAttribute : Attribute
    {
        public EventListenerAttribute(EventPriority priority = EventPriority.Normal)
        {
            Priority = priority;
            Events = new Type[0];
        }

        public EventListenerAttribute(Type @event, EventPriority priority = EventPriority.Normal)
        {
            Priority = priority;
            Events = new[] { @event };
        }

        /// <summary>
        ///     The priority of the event listener.
        /// </summary>
        public EventPriority Priority { get; set; }

        /// <summary>
        ///     The events that the listener is listening to.
        /// </summary>
        public Type[] Events { get; set; }

        /// <summary>
        ///     If set to true, the listener will be called regardless of the <see cref="IEventCancelable.IsCancelled"/>.
        /// </summary>
        public bool IgnoreCancelled { get; set; }

        /// <summary>
        ///     The order of the priority.
        /// </summary>
        public int PriorityOrder { get; set; } = 100;
    }
}