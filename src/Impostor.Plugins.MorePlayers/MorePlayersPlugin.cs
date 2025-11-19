using System;
using System.Threading.Tasks;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Plugins;
using Impostor.Plugins.MorePlayers.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Plugins.MorePlayers
{
    [ImpostorPlugin("com.github.impostor.moreplayers")]
    public class MorePlayersPlugin : PluginBase
    {
        private readonly ILogger<MorePlayersPlugin> _logger;
        private readonly IGameManager _gameManager;
        private readonly MorePlayersConfig _config;

        public MorePlayersPlugin(
            ILogger<MorePlayersPlugin> logger,
            IGameManager gameManager,
            IOptions<MorePlayersConfig> config)
        {
            _logger = logger;
            _gameManager = gameManager;
            _config = config.Value;
        }

        public override async ValueTask EnableAsync()
        {
            _logger.LogInformation("MorePlayers plugin v1.0.0 is being enabled.");
            _logger.LogInformation("Configuration:");
            _logger.LogInformation("  MaxPlayers: {MaxPlayers}", _config.MaxPlayers);
            _logger.LogInformation("  Impostor Ratio: 1 per {Ratio} players", _config.ImpostorRatio);
            _logger.LogInformation("  Calculated Impostors: {Impostors}", _config.CalculatedImpostors);
            _logger.LogInformation("  Auto-create game: {AutoCreate}", _config.AutoCreateGame);

            // Important notes for users
            _logger.LogWarning("IMPORTANT: For >18 players, you MUST set AntiCheat.EnableColorLimitChecks = false in server config!");
            _logger.LogWarning("Without this, players beyond the 18th will not be able to select colors.");

            if (_config.AutoCreateGame)
            {
                await CreateHighCapacityGameAsync();
            }
            else
            {
                _logger.LogInformation("Auto-create disabled. Use IGameManager to create games manually with desired MaxPlayers.");
            }
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("MorePlayers plugin is being disabled.");
            return default;
        }

        private async ValueTask CreateHighCapacityGameAsync()
        {
            try
            {
                var options = new NormalGameOptions
                {
                    MaxPlayers = _config.MaxPlayers,
                    NumImpostors = Math.Min(_config.CalculatedImpostors, 15), // Cap at 15 to be safe
                    PlayerSpeedMod = 1.0f,
                    CrewLightMod = 1.0f,
                    ImpostorLightMod = 1.5f,
                    KillCooldown = 20f,
                    NumCommonTasks = 1,
                    NumLongTasks = 2,
                    NumShortTasks = 3,
                    NumEmergencyMeetings = 1,
                    EmergencyCooldown = 15,
                    DiscussionTime = 30,
                    VotingTime = 120,
                    ConfirmImpostor = true,
                    VisualTasks = true,
                    AnonymousVotes = false,
                    TaskBarUpdate = TaskBarUpdate.Always
                };

                var game = await _gameManager.CreateAsync(options, GameFilterOptions.CreateDefault());

                if (game == null)
                {
                    _logger.LogWarning("Game creation was cancelled by another plugin or system.");
                }
                else
                {
                    game.DisplayName = _config.GameName;
                    await game.SetPrivacyAsync(_config.MakeGamePublic);

                    _logger.LogInformation("Successfully created high-capacity game with code: {GameCode}", game.Code.Code);
                    _logger.LogInformation("  Display Name: {Name}", game.DisplayName);
                    _logger.LogInformation("  Public: {IsPublic}", _config.MakeGamePublic);
                    _logger.LogInformation("  Max Players: {MaxPlayers}", options.MaxPlayers);
                    _logger.LogInformation("  Impostors: {Impostors}", options.NumImpostors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create high-capacity game.");
            }
        }
    }
}
