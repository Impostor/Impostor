using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Localization;
using Impostor.Api.Plugins;
using Impostor.Server.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Plugins
{
    public class PluginLoaderService : IHostedService
    {
        private readonly ILogger<PluginLoaderService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILocalizationRegistry _localizationRegistry;
        private readonly List<PluginInformation> _plugins;
        private readonly List<IDisposable> _localizationRegistrations = new();

        public PluginLoaderService(
            ILogger<PluginLoaderService> logger,
            IServiceProvider serviceProvider,
            ILocalizationRegistry localizationRegistry,
            List<PluginInformation> plugins)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _localizationRegistry = localizationRegistry;
            _plugins = plugins;
        }

        public IReadOnlyList<PluginInformation> Plugins => _plugins;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Loading plugins.");

            foreach (var plugin in _plugins)
            {
                _logger.LogInformation("Enabling plugin {0}.", plugin);

                RegisterLocalization(plugin);

                // Create instance and inject services.
                plugin.Instance = (IPlugin)ActivatorUtilities.CreateInstance(_serviceProvider, plugin.PluginType);

                // Enable plugin.
                await plugin.Instance.EnableAsync();
            }

            _logger.LogInformation(
                _plugins.Count == 1
                    ? "Loaded {0} plugin."
                    : "Loaded {0} plugins.",
                _plugins.Count
            );
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Disable all plugins with a valid instance set.
            // In the case of a failed startup, some can be null.
            foreach (var plugin in _plugins)
            {
                if (plugin.Instance != null)
                {
                    _logger.LogInformation("Disabling plugin {0}.", plugin);

                    // Disable plugin.
                    await plugin.Instance.DisableAsync();
                }
            }

            foreach (var registration in _localizationRegistrations)
            {
                registration.Dispose();
            }

            _localizationRegistrations.Clear();
        }

        private void RegisterLocalization(PluginInformation plugin)
        {
            var assembly = plugin.PluginType.Assembly;

            if (!LocalizationResourceLoader.HasLocalizationResources(assembly))
            {
                return;
            }

            if (_localizationRegistry.TryRegister(new LocalizationCatalog(plugin.Id, assembly), out var registration) && registration != null)
            {
                _localizationRegistrations.Add(registration);
                _logger.LogDebug("Registered localization resources of {Plugin}.", plugin);
            }
        }
    }
}
