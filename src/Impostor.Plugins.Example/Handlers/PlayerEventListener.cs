using System;
using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Innersloth.GameOptions;
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
                    // TODO Rewrite using cosmetics source generator
                    // await playerControl.SetHatAsync((HatType)_random.Next(1, 9));
                    // await playerControl.SetSkinAsync((SkinType)_random.Next(1, 9));
                    // await playerControl.SetPetAsync((PetType)_random.Next(1, 9));

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
                e.Game.Options.NumImpostors = 2;

                if (e.Game.Options is NormalGameOptions normalGameOptions)
                {
                    normalGameOptions.KillCooldown = 0;
                    normalGameOptions.PlayerSpeedMod = 5;
                }

                await e.Game.SyncSettingsAsync();
            }

            if (e.Message == "look")
            {
                await e.PlayerControl.SetColorAsync(ColorType.Pink);
                await e.PlayerControl.SetHatAsync("hat_pk05_Cheese");
                await e.PlayerControl.SetSkinAsync("skin_Police");
                await e.PlayerControl.SetPetAsync("pet_alien1");
            }

            if (e.Message == "snap")
            {
                await e.PlayerControl.NetworkTransform.SnapToAsync(new Vector2(1, 1));
            }

            if (e.Message == "completetasks")
            {
                foreach (var task in e.PlayerControl.PlayerInfo.Tasks)
                {
                    await task.CompleteAsync();
                }
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
            _logger.LogInformation("Player {player} entered the vent in {vent} ({ventId})", e.PlayerControl.PlayerInfo.PlayerName, e.Vent.Name, e.Vent.Id);
        }

        [EventListener]
        public void OnPlayerExitVentEvent(IPlayerExitVentEvent e)
        {
            _logger.LogInformation("Player {player} exited the vent in {vent} ({ventId})", e.PlayerControl.PlayerInfo.PlayerName, e.Vent.Name, e.Vent.Id);
        }

        [EventListener]
        public void OnPlayerVentEvent(IPlayerVentEvent e)
        {
            _logger.LogInformation("Player {player} vented to {vent} ({ventId})", e.PlayerControl.PlayerInfo.PlayerName, e.NewVent.Name, e.NewVent.Id);
        }

        [EventListener]
        public void OnPlayerVoted(IPlayerVotedEvent e)
        {
            _logger.LogDebug("Player {player} voted for {type} {votedFor}", e.PlayerControl.PlayerInfo.PlayerName, e.VoteType, e.VotedFor?.PlayerInfo.PlayerName);
        }

        [EventListener]
        public void OnPlayerCompletedTaskEvent(IPlayerCompletedTaskEvent e)
        {
            _logger.LogInformation("Player {player} completed {task}, {type}, {category}, visual {visual}", e.PlayerControl.PlayerInfo.PlayerName, e.Task.Task.Name, e.Task.Task.Type, e.Task.Task.Category, e.Task.Task.IsVisual);
        }
    }
}
