using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example.Handlers
{
    public class GameEventListener : IEventListener
    {
        private readonly ILogger<GameEventListener> _logger;

        public GameEventListener(ILogger<GameEventListener> logger)
        {
            _logger = logger;
        }

        [EventListener]
        public void OnGameCreated(IGameCreationEvent e)
        {
            _logger.LogInformation("Game creation requested by {client}", e.Client == null ? "a plugin" : e.Client.Name);

            if (e.Client != null)
            {
                var gameCode = GameCode.From(e.Client.Name);

                if (!gameCode.IsInvalid)
                {
                    e.GameCode = gameCode;
                }

                if (e.Client.Name == "dima")
                {
                    e.IsCancelled = true;
                    e.Client.DisconnectAsync(DisconnectReason.Custom, "No you dont >:(");
                }
            }
        }

        [EventListener]
        public void OnGameCreated(IGameCreatedEvent e)
        {
            _logger.LogInformation("Game {code} > created", e.Game.Code);
        }

        [EventListener]
        public void OnGameStarting(IGameStartingEvent e)
        {
            _logger.LogInformation("Game {code} > starting", e.Game.Code);
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            _logger.LogInformation("Game {code} > started", e.Game.Code);

            foreach (var player in e.Game.Players)
            {
                var info = player.Character!.PlayerInfo;

                _logger.LogInformation("- {player} is {role}", info.PlayerName, info.IsImpostor ? "an impostor" : "a crewmate");
            }
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            _logger.LogInformation("Game {code} > ended because {reason}", e.Game.Code, e.GameOverReason);
        }

        [EventListener]
        public void OnGameDestroyed(IGameDestroyedEvent e)
        {
            _logger.LogInformation("Game {code} > destroyed", e.Game.Code);
        }

        [EventListener]
        public void OnGameHostChanged(IGameHostChangedEvent e)
        {
            _logger.LogInformation(
                "Game {code} > changed host from {previous} to {new}",
                e.Game.Code,
                e.PreviousHost.Character?.PlayerInfo.PlayerName,
                e.NewHost != null ? e.NewHost.Character?.PlayerInfo.PlayerName : "none"
            );
        }

        [EventListener]
        public void OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            _logger.LogInformation("Game {code} > {player} joined", e.Game.Code, e.Player.Client.Name);
        }

        [EventListener]
        public void OnPlayerLeftGame(IGamePlayerLeftEvent e)
        {
            _logger.LogInformation("Game {code} > {player} left", e.Game.Code, e.Player.Client.Name);
        }
    }
}
