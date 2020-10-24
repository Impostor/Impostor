using System;
using Impostor.Api.Events;
using Impostor.Api.Events.Net;

namespace Impostor.Plugins.Example.Handlers
{
    public class PlayerEventListener : IEventListener
    {
        [EventListener]
        public void OnPlayerSpawned(PlayerSpawnedEvent e)
        {
            Console.WriteLine(e.PlayerControl.PlayerInfo.PlayerName + " spawned");
        }

        [EventListener]
        public void OnPlayerDestroyed(PlayerDestroyedEvent e)
        {
            Console.WriteLine(e.PlayerControl.PlayerInfo.PlayerName + " destroyed");
        }

        [EventListener]
        public void OnPlayerChat(PlayerChatEvent e)
        {
            Console.WriteLine(e.PlayerControl.PlayerInfo.PlayerName + " said " + e.Message);
        }
    }
}