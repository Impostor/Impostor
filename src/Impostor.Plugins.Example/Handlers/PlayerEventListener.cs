using System;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Innersloth.Customization;

namespace Impostor.Plugins.Example.Handlers
{
    public class PlayerEventListener : IEventListener
    {
        private static readonly Random Random = new Random();

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            Console.WriteLine(e.PlayerControl.PlayerInfo.PlayerName + " spawned");

            // Need to make a local copy because it might be possible that
            // the event gets changed after being handled.
            var clientPlayer = e.ClientPlayer;
            var playerControl = e.PlayerControl;

            Task.Run(async () =>
            {
                Console.WriteLine("Starting player task.");

                // Give the player time to load.
                await Task.Delay(TimeSpan.FromSeconds(3));

                while (clientPlayer.Client.Connection != null &&
                       clientPlayer.Client.Connection.IsConnected)
                {
                    // Modify player properties.
                    await playerControl.SetColorAsync((byte) Random.Next(1, 9));
                    await playerControl.SetHatAsync((uint) Random.Next(1, 9));
                    await playerControl.SetSkinAsync((uint) Random.Next(1, 9));
                    await playerControl.SetPetAsync((uint) Random.Next(1, 9));

                    await Task.Delay(TimeSpan.FromMilliseconds(5000));
                }

                Console.WriteLine("Stopping player task.");
            });
        }

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent e)
        {
            Console.WriteLine(e.PlayerControl.PlayerInfo.PlayerName + " destroyed");
        }

        [EventListener]
        public async ValueTask OnPlayerChat(IPlayerChatEvent e)
        {
            Console.WriteLine(e.PlayerControl.PlayerInfo.PlayerName + " said " + e.Message);

            if (e.Message == "test")
            {
                e.Game.Options.KillCooldown = 0;
                e.Game.Options.NumImpostors = 2;
                e.Game.Options.PlayerSpeedMod = 5;

                await e.Game.SyncSettingsAsync();
            }

            if (e.Message == "look")
            {
                await e.PlayerControl.SetColorAsync(ColorType.Pink);
                await e.PlayerControl.SetHatAsync(HatType.Cheese);
                await e.PlayerControl.SetSkinAsync(SkinType.Police);
                await e.PlayerControl.SetPetAsync(PetType.Ufo);
            }

            await e.PlayerControl.SetNameAsync(e.Message);
            await e.PlayerControl.SendChatAsync(e.Message);
        }
    }
}
