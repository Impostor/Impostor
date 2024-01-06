using System.Threading.Tasks;
using Impostor.Api.Games.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Impostor.Plugins.Example;

[ImpostorPlugin("gg.impostor.example")]
public class ExamplePlugin : PluginBase
{
    private readonly IGameManager _gameManager;
    private readonly ILogger<ExamplePlugin> _logger;

    public ExamplePlugin(ILogger<ExamplePlugin> logger, IGameManager gameManager)
    {
        _logger = logger;
        _gameManager = gameManager;
    }

    public override async ValueTask EnableAsync()
    {
        _logger.LogInformation("Example is being enabled.");

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
        return default;
    }
}
