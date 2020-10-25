using System.Threading.Tasks;

namespace Impostor.Api.Events
{
    public interface IManualEventListener : IEventListener
    {
        public bool CanExecute<T>();

        public ValueTask Execute(IEvent @event);

        EventPriority Priority { get; set; }
    }
}