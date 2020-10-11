using System.Threading.Tasks;

namespace Impostor.Server.Events.Managers
{
    public interface IEventManager
    {
        /// <summary>
        ///     Returns true if an event with the type <see cref="TEvent"/> is registered.
        /// </summary>
        /// <returns>True if the <see cref="TEvent"/> is registered.</returns>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        bool IsRegistered<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        ///     Call all the event listeners for the type <see cref="TEvent"/>.
        /// </summary>
        /// <param name="event">The event argument.</param>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        ValueTask CallAsync<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}