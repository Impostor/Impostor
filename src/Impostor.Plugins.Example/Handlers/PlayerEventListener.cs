using System;
using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Innersloth.Customization;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example.Handlers
{
    public class PlayerEventListener : IEventListener
    {
        private readonly Random _random = new Random();

        private readonly ILogger<PlayerEventListener> _logger;

        public PlayerEventListener(ILogger<PlayerEventListener> logger)
        {
            _logger = logger;
        }

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            _logger.LogInformation("Player {player} > spawned", e.PlayerControl.PlayerInfo.PlayerName);

            // Need to make a local copy because it might be possible that
            // the event gets changed after being handled.
            var clientPlayer = e.ClientPlayer;
            var playerControl = e.PlayerControl;

            Task.Run(async () =>
            {
                _logger.LogDebug("Starting player task");

                // Give the player time to load.
                await Task.Delay(TimeSpan.FromSeconds(3));

                while (clientPlayer.Client.Connection != null && clientPlayer.Client.Connection.IsConnected)
                {
                    // Modify player properties.
                    await playerControl.SetColorAsync((ColorType)_random.Next(1, 9));
                    await playerControl.SetHatAsync((HatType)_random.Next(1, 9));
                    await playerControl.SetSkinAsync((SkinType)_random.Next(1, 9));
                    await playerControl.SetPetAsync((PetType)_random.Next(1, 9));

                    await Task.Delay(TimeSpan.FromMilliseconds(5000));
                }

                _logger.LogDebug("Stopping player task");
            });
        }

        [EventListener]
        public void OnPlayerDestroyed(IPlayerDestroyedEvent e)
        {
            _logger.LogInformation("Player {player} > destroyed", e.PlayerControl.PlayerInfo.PlayerName);
        }

        [EventListener]
        public async ValueTask OnPlayerChat(IPlayerChatEvent e)
        {
            _logger.LogInformation("Player {player} > said {message}", e.PlayerControl.PlayerInfo.PlayerName, e.Message);

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

            if (e.Message == "snap")
            {
                await e.PlayerControl.NetworkTransform.SnapToAsync(new Vector2(1, 1));
            }

            await e.PlayerControl.SetNameAsync(e.Message);
            await e.PlayerControl.SendChatAsync(e.Message);
        }

        [EventListener]
        public void OnPlayerStartMeetingEvent(IPlayerStartMeetingEvent e)
        {
            _logger.LogInformation("Player {player} > started meeting, reason: {reason}", e.PlayerControl.PlayerInfo.PlayerName, e.Body == null ? "Emergency call button" : "Found the body of the player " + e.Body.PlayerInfo.PlayerName);
        }

        [EventListener]
        public void OnPlayerEnterVentEvent(IPlayerEnterVentEvent e)
        {
            _logger.LogInformation("Player {player} entered the vent in {vent}", e.PlayerControl.PlayerInfo.PlayerName, e.Vent.Name);
        }

        [EventListener]
        public void OnPlayerExitVentEvent(IPlayerExitVentEvent e)
        {
            _logger.LogInformation("Player {player} exited the vent in {vent}", e.PlayerControl.PlayerInfo.PlayerName, e.Vent.Name);
        }

        [EventListener]
        public void OnPlayerVentEvent(IPlayerVentEvent e)
        {
            _logger.LogInformation("Player {player} vented to {vent}", e.PlayerControl.PlayerInfo.PlayerName, e.NewVent.Name);
        }
    }
}
