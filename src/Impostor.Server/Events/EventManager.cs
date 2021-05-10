using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ConcurrentDictionary<Type, List<EventHandler>> _cachedEventHandlers;
        private readonly ILogger<EventManager> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EventManager(ILogger<EventManager> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _temporaryEventListeners = new ConcurrentDictionary<Type, TemporaryEventRegister>();
            _cachedEventHandlers = new ConcurrentDictionary<Type, List<EventHandler>>();
        }

        /// <inheritdoc />
        public IDisposable RegisterListener<TListener>(TListener listener, Func<Func<Task>, Task>? invoker = null)
            where TListener : IEventListener
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            var eventListeners = RegisteredEventListener.FromType(listener.GetType());
            var disposes = new List<IDisposable>(eventListeners.Count);

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

                disposes.Add(register.Add(wrappedEventListener));
            }

            if (eventListeners.Count > 0)
            {
                _cachedEventHandlers.TryRemove(typeof(TListener), out _);
            }

            return new MultiDisposable(disposes);
        }

        /// <inheritdoc />
        public bool IsRegistered<TEvent>()
            where TEvent : IEvent
        {
            if (_cachedEventHandlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                return handlers.Count > 0;
            }

            return GetHandlers<TEvent>().Any();
        }

        /// <inheritdoc />
        public async ValueTask CallAsync<T>(T @event)
            where T : IEvent
        {
            try
            {
                if (!_cachedEventHandlers.TryGetValue(typeof(T), out var handlers))
                {
                    handlers = CacheEventHandlers<T>();
                }

                foreach (var (handler, eventListener) in handlers)
                {
                    await eventListener.InvokeAsync(handler, @event, _serviceProvider);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Invocation of event {0} threw an exception.", @event.GetType().Name);
            }
        }

        private List<EventHandler> CacheEventHandlers<TEvent>()
            where TEvent : IEvent
        {
            var handlers = GetHandlers<TEvent>()
                .OrderByDescending(e => e.Listener.Priority)
                .ToList();

            _cachedEventHandlers[typeof(TEvent)] = handlers;

            return handlers;
        }

        /// <summary>
        ///     Get all the event listeners for the given event type.
        /// </summary>
        /// <returns>The event listeners.</returns>
        private IEnumerable<EventHandler> GetHandlers<TEvent>()
            where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            var interfaces = eventType.GetInterfaces();

            foreach (var @interface in interfaces)
            {
                if (_temporaryEventListeners.TryGetValue(@interface, out var cb))
                {
                    foreach (var eventListener in cb.GetEventListeners())
                    {
                        yield return new EventHandler(null, eventListener);
                    }
                }
            }

            foreach (var handler in _serviceProvider.GetServices<IEventListener>())
            {
                if (handler is IManualEventListener manualEventListener && manualEventListener.CanExecute<TEvent>())
                {
                    yield return new EventHandler(handler, new ManualRegisteredEventListener(manualEventListener));
                    continue;
                }

                var events = RegisteredEventListener.FromType(handler.GetType());

                foreach (var eventHandler in events)
                {
                    if (eventHandler.EventType != typeof(TEvent) && !interfaces.Contains(eventHandler.EventType))
                    {
                        continue;
                    }

                    yield return new EventHandler(handler, eventHandler);
                }
            }

            if (_temporaryEventListeners.TryGetValue(eventType, out var cb2))
            {
                foreach (var eventListener in cb2.GetEventListeners())
                {
                    yield return new EventHandler(null, eventListener);
                }
            }
        }
    }
}
