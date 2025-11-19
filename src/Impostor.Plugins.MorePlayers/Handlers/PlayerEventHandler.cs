using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Events;
using Impostor.Api.Events.Player;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.MorePlayers.Handlers
{
    /// <summary>
    /// Handles player-level events to monitor color assignments and other player actions.
    /// </summary>
    public class PlayerEventHandler : IEventListener
    {
        private readonly ILogger<PlayerEventHandler> _logger;

        public PlayerEventHandler(ILogger<PlayerEventHandler> logger)
        {
            _logger = logger;
        }

        [EventListener]
        public void OnPlayerSpawned(IPlayerSpawnedEvent e)
        {
            _logger.LogDebug(
                "Player {PlayerName} spawned in game {GameCode}.",
                e.PlayerControl.PlayerInfo?.PlayerName ?? "Unknown",
                e.Game.Code.Code);
        }

        [EventListener]
        public void OnPlayerSetColor(IPlayerSetColorEvent e)
        {
            var playerName = e.PlayerControl.PlayerInfo?.PlayerName ?? "Unknown";
            var color = e.PlayerControl.PlayerInfo?.CurrentOutfit.Color;
            var gameCode = e.Game.Code.Code;

            // Count how many players have this color
            var playersWithSameColor = e.Game.Players
                .Count(p => p.Character?.PlayerInfo?.CurrentOutfit.Color == color);

            if (playersWithSameColor > 1)
            {
                _logger.LogInformation(
                    "Player {PlayerName} set color to {Color} in game {GameCode}. " +
                    "This color is shared with {Count} player(s).",
                    playerName,
                    color,
                    gameCode,
                    playersWithSameColor);
            }
            else
            {
                _logger.LogDebug(
                    "Player {PlayerName} set color to {Color} in game {GameCode}.",
                    playerName,
                    color,
                    gameCode);
            }
        }

        [EventListener]
        public void OnPlayerChat(IPlayerChatEvent e)
        {
            // Log chat for monitoring purposes in high-capacity games
            if (e.Game.PlayerCount > 15)
            {
                _logger.LogDebug(
                    "[Chat] {PlayerName} in game {GameCode}: {Message}",
                    e.PlayerControl.PlayerInfo?.PlayerName ?? "Unknown",
                    e.Game.Code.Code,
                    e.Message);
            }
        }

        [EventListener]
        public void OnPlayerStartMeeting(IPlayerStartMeetingEvent e)
        {
            var caller = e.PlayerControl.PlayerInfo?.PlayerName ?? "Unknown";
            var body = e.Body?.PlayerInfo?.PlayerName ?? "(Emergency)";

            _logger.LogInformation(
                "Meeting started in game {GameCode} by {Caller}. Body: {Body}. " +
                "{PlayerCount} players in session.",
                e.Game.Code.Code,
                caller,
                body,
                e.Game.PlayerCount);
        }

        [EventListener]
        public void OnPlayerExile(IPlayerExileEvent e)
        {
            var exiled = e.PlayerControl.PlayerInfo?.PlayerName ?? "Unknown";

            _logger.LogInformation(
                "Player {PlayerName} was exiled from game {GameCode}.",
                exiled,
                e.Game.Code.Code);
        }
    }
}
