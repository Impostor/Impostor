using System;

namespace Impostor.Api.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventListenerAttribute : Attribute
    {
        public EventListenerAttribute(EventPriority priority = EventPriority.Normal, EventCallStep callStep = EventCallStep.Post)
        {
            Priority = priority;
            CallStep = callStep;
        }

        public EventListenerAttribute(EventCallStep callStep)
        {
            Priority = EventPriority.Normal;
            CallStep = callStep;
        }
        
        public EventListenerAttribute(Type @event, EventCallStep callStep)
        {
            Event = @event;
            Priority = EventPriority.Normal;
            CallStep = callStep;
        }
        
        public EventListenerAttribute(Type @event, EventPriority priority = EventPriority.Normal, EventCallStep callStep = EventCallStep.Post)
        {
            Event = @event;
            Priority = priority;
            CallStep = callStep;
        }

        /// <summary>
        ///     The priority of the event listener.
        /// </summary>
        public EventPriority Priority { get; set; }
        
        /// <summary>
        ///     The call step of the event listener.
        /// </summary>
        public EventCallStep CallStep { get; set; }

        /// <summary>
        ///     The events that the listener is listening to.
        /// </summary>
        public Type? Event { get; set; }
    }
}