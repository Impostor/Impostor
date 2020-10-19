using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.Events
{
    internal class EventManager : IEventManager
    {
        private readonly ConcurrentDictionary<Type, object> _temporaryEventListeners;
        private readonly IServiceProvider _serviceProvider;

        public EventManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _temporaryEventListeners = new ConcurrentDictionary<Type, object>();
        }

        /// <inheritdoc />
        public IDisposable Register<TEvent>(Func<IServiceProvider, TEvent, ValueTask> callback)
            where TEvent : IEvent
        {
            var register = (TemporaryEventRegister<TEvent>) _temporaryEventListeners.GetOrAdd(
                typeof(TEvent),
                _ => new TemporaryEventRegister<TEvent>());

            return register.Add(callback);
        }

        /// <inheritdoc />
        public IDisposable RegisterListener<TListener>(TListener listener, Func<Func<Task>, Task> invoker = null)
            where TListener : IEventListener
        {
            if (listener == null)
            {
                throw new ArgumentNullException(nameof(listener));
            }

            var registerMethod = typeof(EventManager).GetMethod(nameof(RegisterListenerImpl), BindingFlags.Instance | BindingFlags.NonPublic);
            var methods = RegisteredEventListener.FromType(listener.GetType());
            var disposes = new IDisposable[methods.Count];

            for (var i = 0; i < methods.Count; i++)
            {
                var method = methods[i];

                disposes[i] = (IDisposable) registerMethod!
                    .MakeGenericMethod(method.EventType)
                    .Invoke(this, new object[] { listener, method, invoker });
            }

            return new MultiDisposable(disposes);
        }

        /// <inheritdoc />
        public bool IsRegistered<TEvent>()
            where TEvent : IEvent
        {
            using var scope = _serviceProvider.CreateScope();

            return GetHandlers<TEvent>(_serviceProvider).Any();
        }

        /// <inheritdoc />
        public async ValueTask CallAsync<T>(T @event)
            where T : IEvent
        {
            var scope = _serviceProvider.CreateScope();

            try
            {
                foreach (var (handler, eventListener) in GetHandlers<T>(scope.ServiceProvider))
                {
                    await eventListener.InvokeAsync(handler, @event, scope.ServiceProvider);
                }

                if (_temporaryEventListeners.TryGetValue(typeof(T), out var cb))
                {
                    await ((TemporaryEventRegister<T>) cb).CallAsync(scope.ServiceProvider, @event);
                }
            }
            finally
            {
                scope.Dispose();
            }
        }

        /// <summary>
        ///     Get all the event listeners for the given event type.
        /// </summary>
        /// <param name="services">Current service provider.</param>
        /// <returns>The event listeners.</returns>
        private static IEnumerable<EventHandler> GetHandlers<TEvent>(IServiceProvider services)
            where TEvent : IEvent
        {
            foreach (var handler in services.GetServices<IEventListener>())
            {
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
        }

        private IDisposable RegisterListenerImpl<TEvent>(object obj, RegisteredEventListener listener, Func<Func<Task>, Task> invoker = null)
            where TEvent : IEvent
        {
            return invoker == null
                ? Register<TEvent>((provider, @event) => listener.InvokeAsync(obj, @event, provider))
                : Register<TEvent>((provider, @event) => new ValueTask(invoker(() => listener.InvokeAsync(obj, @event, provider).AsTask())));
        }
    }
}