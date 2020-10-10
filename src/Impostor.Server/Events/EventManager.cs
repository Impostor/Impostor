using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Server.Events.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.Events
{
    internal class EventManager : IEventManager
    {
        private readonly IServiceProvider _serviceProvider;

        public EventManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
    }
}