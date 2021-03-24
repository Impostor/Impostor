using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.Events.Register
{
    internal class RegisteredEventListener : IRegisteredEventListener
    {
        private static readonly PropertyInfo IsCancelledProperty = typeof(IEventCancelable).GetProperty(nameof(IEventCancelable.IsCancelled))!;

        private static readonly ConcurrentDictionary<Type, RegisteredEventListener[]> Instances = new ConcurrentDictionary<Type, RegisteredEventListener[]>();
        private readonly Func<object?, object, IServiceProvider, ValueTask> _invoker;
        private readonly Type _eventListenerType;

        public RegisteredEventListener(Type eventType, MethodInfo method, EventListenerAttribute attribute, Type eventListenerType)
        {
            EventType = eventType;
            _eventListenerType = eventListenerType;
            Priority = attribute.Priority;
            IgnoreCancelled = attribute.IgnoreCancelled;
            Method = method.GetFriendlyName(showParameters: false);
            _invoker = CreateInvoker(method, attribute.IgnoreCancelled);
        }

        public Type EventType { get; }

        public EventPriority Priority { get; }

        public int PriorityOrder { get; set; }

        public bool IgnoreCancelled { get; }

        public string Method { get; }

        public static IReadOnlyList<RegisteredEventListener> FromType(Type type)
        {
            return Instances.GetOrAdd(type, t =>
            {
                return t.GetMethods()
                    .Where(m => !m.IsStatic && m.GetCustomAttributes(typeof(EventListenerAttribute), false).Any())
                    .SelectMany(m => FromMethod(t, m))
                    .ToArray();
            });
        }

        public static IEnumerable<RegisteredEventListener> FromMethod(Type listenerType, MethodInfo methodType)
        {
            // Get the return type.
            var returnType = methodType.ReturnType;

            if (returnType != typeof(void) && returnType != typeof(ValueTask))
            {
                throw new InvalidOperationException($"The method {methodType.GetFriendlyName()} does not return void or ValueTask.");
            }

            // Register the event.
            foreach (var attribute in methodType.GetCustomAttributes<EventListenerAttribute>(false))
            {
                var eventType = attribute.Event;

                if (eventType == null)
                {
                    if (methodType.GetParameters().Length == 0 || !typeof(IEvent).IsAssignableFrom(methodType.GetParameters()[0].ParameterType))
                    {
                        throw new InvalidOperationException($"The first parameter of the method {methodType.GetFriendlyName()} should be the type {nameof(IEvent)}.");
                    }

                    eventType = methodType.GetParameters()[0].ParameterType;
                }

                yield return new RegisteredEventListener(eventType, methodType, attribute, listenerType);
            }
        }

        public ValueTask InvokeAsync(object? eventHandler, object @event, IServiceProvider provider)
        {
            return _invoker(eventHandler, @event, provider);
        }

        private Func<object?, object, IServiceProvider, ValueTask> CreateInvoker(MethodInfo method, bool ignoreCancelled)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var eventParameter = Expression.Parameter(typeof(object), "event");
            var provider = Expression.Parameter(typeof(IServiceProvider), "provider");
            var @event = Expression.Convert(eventParameter, EventType);

            var getRequiredService = typeof(ServiceProviderServiceExtensions)
                .GetMethod("GetRequiredService", new[] { typeof(IServiceProvider) });

            if (getRequiredService == null)
            {
                throw new InvalidOperationException("The method GetRequiredService could not be found.");
            }

            var methodArguments = method.GetParameters();
            var arguments = new Expression[methodArguments.Length];

            for (var i = 0; i < methodArguments.Length; i++)
            {
                var methodArgument = methodArguments[i];

                if (typeof(IEvent).IsAssignableFrom(methodArgument.ParameterType)
                    && methodArgument.ParameterType.IsAssignableFrom(EventType))
                {
                    arguments[i] = @event;
                }
                else
                {
                    arguments[i] = Expression.Call(
                        getRequiredService.MakeGenericMethod(methodArgument.ParameterType),
                        provider);
                }
            }

            var returnTarget = Expression.Label(typeof(ValueTask));

            Expression invoke = Expression.Call(Expression.Convert(instance, _eventListenerType), method, arguments);

            if (method.ReturnType == typeof(void))
            {
                if (!ignoreCancelled && typeof(IEventCancelable).IsAssignableFrom(EventType))
                {
                    invoke = Expression.Block(
                        Expression.IfThenElse(
                            Expression.Property(@event, IsCancelledProperty),
                            Expression.Return(returnTarget, Expression.Default(typeof(ValueTask))),
                            Expression.Block(
                                invoke,
                                Expression.Return(returnTarget, Expression.Default(typeof(ValueTask))))),
                        Expression.Label(returnTarget, Expression.Default(typeof(ValueTask))));
                }
                else
                {
                    invoke = Expression.Block(
                        invoke,
                        Expression.Label(returnTarget, Expression.Default(typeof(ValueTask))));
                }
            }
            else if (method.ReturnType == typeof(ValueTask))
            {
                if (!ignoreCancelled && typeof(IEventCancelable).IsAssignableFrom(EventType))
                {
                    invoke = Expression.Block(
                        Expression.IfThenElse(
                            Expression.Property(@event, IsCancelledProperty),
                            Expression.Return(returnTarget, Expression.Default(typeof(ValueTask))),
                            Expression.Return(returnTarget, invoke)),
                        Expression.Label(returnTarget, Expression.Default(typeof(ValueTask))));
                }
            }
            else
            {
                throw new InvalidOperationException($"The method {method.GetFriendlyName()} must return void or ValueTask.");
            }

            return Expression.Lambda<Func<object?, object, IServiceProvider, ValueTask>>(invoke, instance, eventParameter, provider)
                .Compile();
        }
    }
}
