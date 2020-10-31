// using System.Threading.Tasks;
// using BenchmarkDotNet.Attributes;
// using Impostor.Api.Events;
// using Impostor.Api.Events.Managers;
// using Impostor.Server.Events;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Impostor.Benchmarks.Tests
// {
//     public class EventManagerBenchmark
//     {
//         private IEventManager _eventManager;
//         private IGameEvent _event;
//
//         [GlobalSetup]
//         public void Setup()
//         {
//             var services = new ServiceCollection();
//
//             services.AddLogging();
//             services.AddSingleton<IEventManager, EventManager>();
//
//             _event = new GameStartedEvent(null);
//             _eventManager = services.BuildServiceProvider().GetRequiredService<IEventManager>();
//             _eventManager.RegisterListener(new EventListener());
//             _eventManager.RegisterListener(new EventListener());
//             _eventManager.RegisterListener(new EventListener());
//             _eventManager.RegisterListener(new EventListener());
//             _eventManager.RegisterListener(new EventListener());
//         }
//
//         [Benchmark]
//         public async Task Run_1()
//         {
//             for (var i = 0; i < 1; i++)
//             {
//                 await _eventManager.CallAsync(_event);
//             }
//         }
//
//         [Benchmark]
//         public async Task Run_1000()
//         {
//             for (var i = 0; i < 1000; i++)
//             {
//                 await _eventManager.CallAsync(_event);
//             }
//         }
//
//         [Benchmark]
//         public async Task Run_10000()
//         {
//             for (var i = 0; i < 10000; i++)
//             {
//                 await _eventManager.CallAsync(_event);
//             }
//         }
//
//         [Benchmark]
//         public async Task Run_100000()
//         {
//             for (var i = 0; i < 100000; i++)
//             {
//                 await _eventManager.CallAsync(_event);
//             }
//         }
//
//         private class EventListener : IEventListener
//         {
//             [EventListener]
//             public void OnGameStarted(IGameStartedEvent e)
//             {
//
//             }
//         }
//     }
// }
