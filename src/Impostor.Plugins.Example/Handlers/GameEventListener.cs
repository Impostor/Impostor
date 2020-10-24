using System;
using Impostor.Api.Events;

namespace Impostor.Plugins.Example.Handlers
{
    public class GameEventListener : IEventListener
    {
        [EventListener]
        public void OnGameCreated(GameCreatedEvent e)
        {
            Console.WriteLine("Game > created");
        }

        [EventListener]
        public void OnGameStarting(GameStartingEvent e)
        {
            Console.WriteLine("Game > starting");
        }

        [EventListener]
        public void OnGameStarted(GameStartedEvent e)
        {
            Console.WriteLine("Game > started");

            foreach (var player in e.Game.Players)
            {
                var info = player.Character.PlayerInfo;

                Console.WriteLine($"- {info.PlayerName} {info.IsImpostor}");
            }
        }

        [EventListener]
        public void OnGameEnded(GameEndedEvent e)
        {
            Console.WriteLine("Game > ended");
        }

        [EventListener]
        public void OnGameDestroyed(GameDestroyedEvent e)
        {
            Console.WriteLine("Game > destroyed");
        }

        [EventListener]
        public void OnPlayerJoined(PlayerJoinedGameEvent e)
        {
            Console.WriteLine("Player joined a game.");
        }

        [EventListener]
        public void OnPlayerLeftGame(PlayerLeftGameEvent e)
        {
            Console.WriteLine("Player left a game.");
        }
    }
}