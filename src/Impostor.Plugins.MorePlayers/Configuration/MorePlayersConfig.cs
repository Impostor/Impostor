using System.ComponentModel.DataAnnotations;

namespace Impostor.Plugins.MorePlayers.Configuration
{
    public class MorePlayersConfig
    {
        /// <summary>
        /// Maximum number of players allowed in games created by this plugin.
        /// Default: 20 (conservative starting point).
        /// Theoretical max: 255 (byte limit).
        /// Recommended: 20-30 for best balance.
        /// </summary>
        [Range(10, 255)]
        public byte MaxPlayers { get; set; } = 20;

        /// <summary>
        /// Suggested number of impostors based on player count.
        /// Formula: ceiling(MaxPlayers / ImpostorRatio).
        /// Default: 5 means 1 impostor per 5 players (20 players = 4 impostors).
        /// </summary>
        [Range(3, 10)]
        public int ImpostorRatio { get; set; } = 5;

        /// <summary>
        /// Whether to automatically create a high-capacity game on plugin startup.
        /// Set to false if you want to manage games manually.
        /// </summary>
        public bool AutoCreateGame { get; set; } = true;

        /// <summary>
        /// Name of the automatically created game (if AutoCreateGame is true).
        /// </summary>
        public string GameName { get; set; } = "High Capacity Game";

        /// <summary>
        /// Whether to make the automatically created game public.
        /// </summary>
        public bool MakeGamePublic { get; set; } = false;

        /// <summary>
        /// Gets the calculated number of impostors based on MaxPlayers and ImpostorRatio.
        /// </summary>
        public int CalculatedImpostors => (int)System.Math.Ceiling((double)MaxPlayers / ImpostorRatio);
    }
}
