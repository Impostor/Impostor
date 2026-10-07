using System.Threading.Tasks;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Localization;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example
{
    [ImpostorPlugin("gg.impostor.example")]
    public class ExamplePlugin : PluginBase
    {
        private readonly ILogger<ExamplePlugin> _logger;
        private readonly IGameManager _gameManager;
        private readonly ILocalizationRegistry _localizationRegistry;
        private ILocalizationRegistration _localizationRegistration;

        public ExamplePlugin(ILogger<ExamplePlugin> logger, IGameManager gameManager, ILocalizationRegistry localizationRegistry)
        {
            _logger = logger;
            _gameManager = gameManager;
            _localizationRegistry = localizationRegistry;
        }

        public override async ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");

            if (!_localizationRegistry.TryRegister(new LocalizationCatalog(
                "gg.impostor.example",
                typeof(ExamplePlugin).Assembly,
                "Impostor.Plugins.Example.Localization"), out _localizationRegistration))
            {
                _logger.LogWarning("Could not register Example localization resources");
            }

            var game = await _gameManager.CreateAsync(new NormalGameOptions(), GameFilterOptions.CreateDefault());
            if (game == null)
            {
                _logger.LogWarning("Example game creation was cancelled");
            }
            else
            {
                game.DisplayName = "Example game";
                await game.SetPrivacyAsync(true);

                _logger.LogInformation("Created game {0}.", game.Code.Code);
            }
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            _localizationRegistration?.Dispose();
            return default;
        }
    }
}
