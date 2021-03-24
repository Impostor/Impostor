using System.Threading.Tasks;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example
{
    [ImpostorPlugin(
        package: "gg.impostor.example",
        name: "Example",
        author: "AeonLucid",
        version: "1.0.0")]
    public class ExamplePlugin : PluginBase
    {
        private readonly ILogger<ExamplePlugin> _logger;
        private readonly IGameManager _gameManager;

        public ExamplePlugin(ILogger<ExamplePlugin> logger, IGameManager gameManager)
        {
            _logger = logger;
            _gameManager = gameManager;
        }

        public override async ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");

            var game = await _gameManager.CreateAsync(new GameOptionsData());
            game.DisplayName = "Example game";
            await game.SetPrivacyAsync(true);

            _logger.LogInformation("Created game {0}.", game.Code.Code);
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            return default;
        }
    }
}
