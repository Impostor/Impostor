using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Plugins;

public class PluginLoaderService(
    ILogger<PluginLoaderService> logger,
    IServiceProvider serviceProvider,
    List<PluginInformation> plugins)
    : IHostedService
{
    public IReadOnlyList<PluginInformation> Plugins
    {
        get => plugins;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Loading plugins.");

        foreach (var plugin in plugins)
        {
            logger.LogInformation("Enabling plugin {0}.", plugin);

            // Create instance and inject services.
            plugin.Instance = (IPlugin)ActivatorUtilities.CreateInstance(serviceProvider, plugin.PluginType);

            // Enable plugin.
            await plugin.Instance.EnableAsync();
        }

        logger.LogInformation(
            plugins.Count == 1
                ? "Loaded {0} plugin."
                : "Loaded {0} plugins.",
            plugins.Count
        );
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Disable all plugins with a valid instance set.
        // In the case of a failed startup, some can be null.
        foreach (var plugin in plugins)
        {
            if (plugin.Instance == null)
            {
                continue;
            }

            logger.LogInformation("Disabling plugin {0}.", plugin);

            // Disable plugin.
            await plugin.Instance.DisableAsync();
        }
    }
}
