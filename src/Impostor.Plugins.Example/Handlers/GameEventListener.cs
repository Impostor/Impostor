using System;
using Impostor.Api.Events;

namespace Impostor.Plugins.Example.Handlers
{
    public class GameEventListener : IEventListener
    {
        [EventListener]
        public void OnGameCreated(GameCreatedEvent e)
        {
            Console.WriteLine("Game was created.");
        }

        [EventListener]
        public void OnGameDestroyed(GameDestroyedEvent e)
        {
            Console.WriteLine("Game was destroyed.");
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