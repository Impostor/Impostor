using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.Events
{
    internal class RegisteredEventListener
    {
        private static readonly ConcurrentDictionary<Type, RegisteredEventListener[]> Instances = new ConcurrentDictionary<Type, RegisteredEventListener[]>();
        private readonly Func<object, object, IServiceProvider, ValueTask> _invoker;
        private readonly Type _eventListenerType;

        public RegisteredEventListener(Type eventType, MethodInfo method, EventListenerAttribute attribute, Type eventListenerType)
        {
            EventType = eventType;
            _eventListenerType = eventListenerType;
            Priority = attribute.Priority;
            PriorityOrder = attribute.PriorityOrder;
            IgnoreCancelled = attribute.IgnoreCancelled;
            Method = method.GetFriendlyName(showParameters: false);
            _invoker = CreateInvoker(method, attribute.IgnoreCancelled);
        }

        public Type EventType { get; }

        public EventPriority Priority { get; }

        public int PriorityOrder { get; set; }

        public bool IgnoreCancelled { get; }

        public string Method { get; }

        public ValueTask InvokeAsync(object eventHandler, object @event, IServiceProvider provider)
        {
            return _invoker(eventHandler, @event, provider);
        }

        private Func<object, object, IServiceProvider, ValueTask> CreateInvoker(MethodInfo method, bool ignoreCancelled)
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

                if (methodArgument.ParameterType == EventType)
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
                            Expression.Property(@event, nameof(IEventCancelable.IsCancelled)),
                            Expression.Return(returnTarget, Expression.Constant(Task.CompletedTask)),
                            Expression.Block(
                                invoke,
                                Expression.Return(returnTarget, Expression.Constant(Task.CompletedTask)))),
                        Expression.Label(returnTarget, Expression.Constant(Task.CompletedTask)));
                }
                else
                {
                    invoke = Expression.Block(
                        invoke,
                        Expression.Label(returnTarget, Expression.Constant(Task.CompletedTask)));
                }
            }
            else if (method.ReturnType == typeof(ValueTask))
            {
                if (!ignoreCancelled && typeof(IEventCancelable).IsAssignableFrom(EventType))
                {
                    invoke = Expression.Block(
                        Expression.IfThenElse(
                            Expression.Property(@event, nameof(IEventCancelable.IsCancelled)),
                            Expression.Return(returnTarget, Expression.Constant(Task.CompletedTask)),
                            Expression.Return(returnTarget, invoke)),
                        Expression.Label(returnTarget, Expression.Constant(Task.CompletedTask)));
                }
            }
            else
            {
                throw new InvalidOperationException($"The method {method.GetFriendlyName()} must return void or ValueTask.");
            }

            return Expression.Lambda<Func<object, object, IServiceProvider, ValueTask>>(invoke,  instance, eventParameter, provider)
                .Compile();
        }

        public static IEnumerable<RegisteredEventListener> FromType(Type type)
        {
            return Instances.GetOrAdd(type, t =>
            {
                return t.GetMethods()
                    .Where(m => !m.IsStatic && m.GetCustomAttribute(typeof(EventListenerAttribute), false) != null)
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
            var attribute = methodType.GetCustomAttribute<EventListenerAttribute>(false);

            if (attribute == null)
            {
                yield break;
            }

            Type[] eventTypes;

            if (attribute.Events.Length == 0)
            {
                if (methodType.GetParameters().Length == 0 || !typeof(IEvent).IsAssignableFrom(methodType.GetParameters()[0].ParameterType))
                {
                    throw new InvalidOperationException($"The first parameter of the method {methodType.GetFriendlyName()} should be the type {nameof(IEvent)}.");
                }

                eventTypes = new[] { methodType.GetParameters()[0].ParameterType };
            }
            else
            {
                eventTypes = attribute.Events;
            }

            foreach (var eventType in eventTypes)
            {
                var listener = new RegisteredEventListener(eventType, methodType, attribute, listenerType);

                yield return listener;
            }
        }
    }
}