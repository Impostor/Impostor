using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.MorePlayers.Handlers
{
    /// <summary>
    /// Handles game-level events to monitor high-capacity games.
    /// </summary>
    public class GameEventHandler : IEventListener
    {
        private readonly ILogger<GameEventHandler> _logger;

        public GameEventHandler(ILogger<GameEventHandler> logger)
        {
            _logger = logger;
        }

        [EventListener]
        public void OnGameStarted(IGameStartedEvent e)
        {
            var playerCount = e.Game.PlayerCount;
            var maxPlayers = e.Game.Options.MaxPlayers;

            _logger.LogInformation(
                "Game {GameCode} started with {PlayerCount}/{MaxPlayers} players and {Impostors} impostor(s).",
                e.Game.Code.Code,
                playerCount,
                maxPlayers,
                e.Game.Options.NumImpostors);

            if (playerCount > 18)
            {
                _logger.LogInformation(
                    "Game {GameCode} has more than 18 players - color sharing is active.",
                    e.Game.Code.Code);
            }
        }

        [EventListener]
        public void OnGameEnded(IGameEndedEvent e)
        {
            _logger.LogInformation(
                "Game {GameCode} ended with reason: {Reason}.",
                e.Game.Code.Code,
                e.GameOverReason);
        }

        [EventListener]
        public void OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            var playerCount = e.Game.PlayerCount;
            var maxPlayers = e.Game.Options.MaxPlayers;

            _logger.LogDebug(
                "Player {PlayerName} joined game {GameCode} ({PlayerCount}/{MaxPlayers}).",
                e.ClientPlayer.Client.Name,
                e.Game.Code.Code,
                playerCount,
                maxPlayers);

            // Warn when approaching or exceeding the 18 color limit
            if (playerCount == 18)
            {
                _logger.LogWarning(
                    "Game {GameCode} has reached 18 players - all colors are now in use. " +
                    "Ensure AntiCheat.EnableColorLimitChecks = false for more players to join.",
                    e.Game.Code.Code);
            }
            else if (playerCount > 18)
            {
                _logger.LogInformation(
                    "Game {GameCode} now has {PlayerCount} players - color sharing is required.",
                    e.Game.Code.Code,
                    playerCount);
            }
        }

        [EventListener]
        public void OnPlayerLeft(IGamePlayerLeftEvent e)
        {
            _logger.LogDebug(
                "Player {PlayerName} left game {GameCode} ({PlayerCount}/{MaxPlayers}).",
                e.ClientPlayer.Client.Name,
                e.Game.Code.Code,
                e.Game.PlayerCount,
                e.Game.Options.MaxPlayers);
        }
    }
}
