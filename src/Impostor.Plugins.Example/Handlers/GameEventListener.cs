using Impostor.Api.Events;
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
