using System;
using Impostor.Api.Events;

namespace Impostor.Plugins.Example.Handlers
{
    public class GameEventListener : IEventListener
    {
        [EventListener]
        public void OnGameCreated(IGameCreatedEvent e)
        {
            Console.WriteLine("Game > created");
        }

        [EventListener]
        public void OnGameStarting(IGameStartingEvent e)
        {
            Console.WriteLine("Game > starting");
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            Console.WriteLine("Game > started");

            foreach (var player in e.Game.Players)
            {
                var info = player.Character.PlayerInfo;

                Console.WriteLine($"- {info.PlayerName} {info.IsImpostor}");
            }
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            Console.WriteLine("Game > ended");
        }

        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent e)
        {
            Console.WriteLine("Game > destroyed");
        }

        [EventListener]
        public void OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            Console.WriteLine("Player joined a game.");
        }

        [EventListener]
        public void OnPlayerLeftGame(IGamePlayerLeftEvent e)
        {
            Console.WriteLine("Player left a game.");
        }
    }
}
