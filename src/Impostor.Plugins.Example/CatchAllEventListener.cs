using System;
using System.Threading.Tasks;
using Impostor.Api.Events;

namespace Impostor.Plugins.Example
{
    public class CatchAllEventListener : IManualEventListener
    {
        public EventPriority Priority { get; set; }

        public bool CanExecute<T>()
        {
            return true;
        }

        public ValueTask Execute(IEvent @event)
        {
            Console.WriteLine($"Received event type: {@event.GetType()}");
            return default;
        }

    }
}