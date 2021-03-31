using System;
using System.Threading.Tasks;

namespace Impostor.Api.Events.Managers
{
    public interface IEventManager
    {
        /// <summary>
        ///     Register a temporary event listener.
        /// </summary>
        /// <param name="listener">Event listener.</param>
        /// <param name="invoker">Middleware between the events, which can be used to swap to the correct thread dispatcher.</param>
        /// <returns>Disposable that unregisters the callback from the event manager.</returns>
        /// <typeparam name="TListener">Type of the event listener.</typeparam>
        IDisposable RegisterListener<TListener>(TListener listener, Func<Func<Task>, Task>? invoker = null)
            where TListener : IEventListener;

        /// <summary>
        ///     Returns true if an event with the type <typeparamref name="TEvent" /> is registered.
        /// </summary>
        /// <returns>True if the <typeparamref name="TEvent" /> is registered.</returns>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        bool IsRegistered<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        ///     Call all the event listeners for the type <typeparamref name="TEvent" />.
        /// </summary>
        /// <param name="event">The event argument.</param>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <returns>A <see cref="ValueTask" /> representing the asynchronous operation.</returns>
        ValueTask CallAsync<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}
