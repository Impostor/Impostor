using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Managers;
using Impostor.Server.Events;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Impostor.Tests.Events
{
    public class EventManagerTests
    {
        public static readonly IEnumerable<object[]> TestModes = new[]
        {
            new object[] { TestMode.Service },
            new object[] { TestMode.Temporary },
        };

        [Theory]
        [MemberData(nameof(TestModes))]
        public async Task CallEvent(TestMode mode)
        {
            var listener = new EventListener();
            var eventManager = CreatEventManager(mode, listener);

            await eventManager.CallAsync(new SetValueEvent(1));

            Assert.Equal(1, listener.Value);
        }

        [Theory]
        [MemberData(nameof(TestModes))]
        public async Task CallPriority(TestMode mode)
        {
            var listener = new PriorityEventListener();
            var eventManager = CreatEventManager(mode, listener);

            await eventManager.CallAsync(new SetValueEvent(1));

            Assert.Equal(new[]
            {
                EventPriority.Monitor,
                EventPriority.Highest,
                EventPriority.High,
                EventPriority.Normal,
                EventPriority.Low,
                EventPriority.Lowest,
            }, listener.Priorities);
        }

        [Theory]
        [MemberData(nameof(TestModes))]
        public async Task CancelEvent(TestMode mode)
        {
            var listener = new EventListener();
            var eventManager = CreatEventManager(
                mode,
                new CancelAtHighEventListener(),
                listener
            );

            await eventManager.CallAsync(new SetValueEvent(1));

            Assert.Equal(0, listener.Value);
        }

        [Theory]
        [MemberData(nameof(TestModes))]
        public async Task CancelPriority(TestMode mode)
        {
            var listener = new PriorityEventListener();
            var eventManager = CreatEventManager(
                mode,
                new CancelAtHighEventListener(),
                listener
            );

            await eventManager.CallAsync(new SetValueEvent(1));

            Assert.Equal(new[]
            {
                EventPriority.Monitor,
                EventPriority.Highest,
            }, listener.Priorities);
        }

        private static IEventManager CreatEventManager(TestMode mode, params IEventListener[] listeners)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IEventManager, EventManager>();

            if (mode == TestMode.Service)
            {
                foreach (var listener in listeners)
                {
                    services.AddSingleton(listener);
                }
            }

            var eventManager = services.BuildServiceProvider().GetRequiredService<IEventManager>();

            if (mode == TestMode.Temporary)
            {
                foreach (var listener in listeners)
                {
                    eventManager.RegisterListener(listener);
                }
            }

            return eventManager;
        }

        public enum TestMode
        {
            Service,
            Temporary,
        }

        public interface ISetValueEvent : IEventCancelable
        {
            int Value { get; }
        }

        public class SetValueEvent : ISetValueEvent
        {
            public SetValueEvent(int value)
            {
                Value = value;
            }

            public int Value { get; }

            public bool IsCancelled { get; set; }
        }

        private class CancelAtHighEventListener : IEventListener
        {
            [EventListener(Priority = EventPriority.High)]
            public void OnSetCalled(ISetValueEvent e) => e.IsCancelled = true;
        }

        private class EventListener : IEventListener
        {
            public int Value { get; private set; }

            [EventListener]
            public void OnSetCalled(ISetValueEvent e) => Value = e.Value;
        }

        private class PriorityEventListener : IEventListener
        {
            public List<EventPriority> Priorities { get; } = new List<EventPriority>();

            [EventListener(EventPriority.Lowest)]
            public void OnLowest(ISetValueEvent e) => Priorities.Add(EventPriority.Lowest);

            [EventListener(EventPriority.Low)]
            public void OnLow(ISetValueEvent e) => Priorities.Add(EventPriority.Low);

            [EventListener]
            public void OnNormal(ISetValueEvent e) => Priorities.Add(EventPriority.Normal);

            [EventListener(EventPriority.High)]
            public void OnHigh(ISetValueEvent e) => Priorities.Add(EventPriority.High);

            [EventListener(EventPriority.Highest)]
            public void OnHighest(ISetValueEvent e) => Priorities.Add(EventPriority.Highest);

            [EventListener(EventPriority.Monitor)]
            public void OnMonitor(ISetValueEvent e) => Priorities.Add(EventPriority.Monitor);
        }
    }
}
