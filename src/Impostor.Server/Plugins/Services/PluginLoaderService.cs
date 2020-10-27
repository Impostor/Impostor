using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Plugins;
using Impostor.Api.Plugins.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Plugins.Services
{
    public class PluginLoaderService : IHostedService
    {
        private readonly ILogger<PluginLoaderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPluginManager _pluginManager;

        public PluginLoaderService(ILogger<PluginLoaderService> logger, IServiceProvider serviceProvider, IPluginManager pluginManager)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _pluginManager = pluginManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Loading plugins.");

            foreach (var plugin in _pluginManager.Plugins.OfType<PluginInformation>())
            {
                _logger.LogInformation("Enabling plugin {0}.", plugin);

                // Create instance and inject services.
                plugin.Instance = (IPlugin) ActivatorUtilities.CreateInstance(_serviceProvider, plugin.PluginType);

                // Enable plugin.
                await plugin.Instance.EnableAsync();
            }

            _logger.LogInformation(
                _pluginManager.Plugins.Count == 1
                    ? "Loaded {0} plugin."
                    : "Loaded {0} plugins.", _pluginManager.Plugins.Count);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Disable all plugins with a valid instance set.
            // In the case of a failed startup, some can be null.
            foreach (var plugin in _pluginManager.Plugins.OfType<PluginInformation>())
            {
                if (plugin.Instance == null)
                {
                    continue;
                }

                _logger.LogInformation("Disabling plugin {0}.", plugin);

                // Disable plugin.
                await plugin.Instance.DisableAsync();

                plugin.Instance = null;
            }
        }
    }
}
