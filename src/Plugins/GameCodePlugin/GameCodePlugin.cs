using System.Text.RegularExpressions;
using Impostor.Api.Events;
using Impostor.Api.Games;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameCodePlugin;

[ImpostorPlugin("GameCodePlugin.Impostor.Next")]
public sealed class GameCodePlugin(GameCodeStateManager stateManager, IHostEnvironment env, ILogger<GameCodePlugin> logger) : IPlugin
{
    public async ValueTask EnableAsync()
    {
        logger.LogInformation("GameCodePlugin enabled!");
        var root = env.ContentRootPath;
        try
        {
            await stateManager.LoadCodeAsync(new DirectoryInfo(Path.Combine(root, "GameCode")));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to load game code");
        }
    }
    
}
