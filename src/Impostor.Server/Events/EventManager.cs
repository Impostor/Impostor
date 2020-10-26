using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Managers;
using Impostor.Server.Events.Register;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Events
{
    internal class EventManager : IEventManager
    {
        private readonly ConcurrentDictionary<Type, TemporaryEventRegister> _temporaryEventListeners;
        private readonly ILogger<EventManager> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EventManager(ILogger<EventManager> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _temporaryEventListeners = new ConcurrentDictionary<Type, TemporaryEventRegister>();
        }

        /// <inheritdoc />
        public IDisposable RegisterListener<TListener>(TListener listener, Func<Func<Task>, Task> invoker = null)
            where TListener : IEventListener
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            var eventListeners = RegisteredEventListener.FromType(listener.GetType());
            var disposes = new IDisposable[eventListeners.Count];

            foreach (var eventListener in eventListeners)
            {
                IRegisteredEventListener wrappedEventListener = new WrappedRegisteredEventListener(eventListener, listener);

                if (invoker != null)
                {
                    wrappedEventListener = new InvokedRegisteredEventListener(wrappedEventListener, invoker);
                }

                var register = _temporaryEventListeners.GetOrAdd(
                    wrappedEventListener.EventType,
                    _ => new TemporaryEventRegister());

                register.Add(wrappedEventListener);
            }

            return new MultiDisposable(disposes);
        }

        /// <inheritdoc />
        public bool IsRegistered<TEvent>()
            where TEvent : IEvent
        {
            return GetHandlers<TEvent>(_serviceProvider).Any();
        }

        /// <inheritdoc />
        public async ValueTask CallAsync<T>(T @event)
            where T : IEvent
        {
            try
            {
                foreach (var (handler, eventListener) in GetHandlers<T>(_serviceProvider)
                    .OrderByDescending(e => e.Listener.Priority))
                {
                    await eventListener.InvokeAsync(handler, @event, _serviceProvider);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Invocation of event {0} threw an exception.", @event.GetType().Name);
            }
        }

        /// <summary>
        ///     Get all the event listeners for the given event type.
        /// </summary>
        /// <param name="services">Current service provider.</param>
        /// <returns>The event listeners.</returns>
        private IEnumerable<EventHandler> GetHandlers<TEvent>(IServiceProvider services)
            where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            var interfaces = eventType.GetInterfaces();
            if (interfaces.Length > 0 && _temporaryEventListeners.TryGetValue(interfaces[0], out var cb))
            {
                foreach (var eventListener in cb.GetEventListeners())
                {
                    yield return new EventHandler(null, eventListener);
                }
            }

            foreach (var handler in services.GetServices<IEventListener>())
            {
                if (handler is IManualEventListener manualEventListener && manualEventListener.CanExecute<TEvent>())
                {
                    yield return new EventHandler(handler, new ManualRegisteredEventListener(manualEventListener));
                    continue;
                }

                var events = RegisteredEventListener.FromType(handler.GetType());

                foreach (var eventHandler in events)
                {
                    if (eventHandler.EventType != typeof(TEvent))
                    {
                        continue;
                    }

                    yield return new EventHandler(handler, eventHandler);
                }
            }

            if (_temporaryEventListeners.TryGetValue(eventType, out cb))
            {
                foreach (var eventListener in cb.GetEventListeners())
                {
                    yield return new EventHandler(null, eventListener);
                }
            }
        }
    }
}
