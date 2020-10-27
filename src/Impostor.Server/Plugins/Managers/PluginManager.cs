using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Impostor.Api.Plugins;
using Impostor.Api.Plugins.Managers;
using Impostor.Server.Plugins.Config;
using Impostor.Server.Plugins.Proxies;
using Impostor.Server.Plugins.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Plugins.Managers
{
    public class PluginManager : IPluginManager
    {
        private readonly ILogger<PluginManager> _logger;
        private readonly IOptions<PluginConfig> _pluginConfig;
        private readonly IServiceProvider _serviceProvider;
        private readonly ServiceProxyCollection _serviceProxyCollection;
        private IHost? _currentHost;
        private PluginAssemblyLoadContext? _currentContext;

        public PluginManager(
            IOptions<PluginConfig> pluginConfig,
            ILogger<PluginManager> logger,
            IServiceProvider serviceProvider,
            ServiceProxyCollection serviceProxyCollection)
        {
            _pluginConfig = pluginConfig;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _serviceProxyCollection = serviceProxyCollection;
        }

        public IReadOnlyList<IPluginInformation> Plugins { get; private set; } = new List<IPluginInformation>();

        public async ValueTask ReloadAsync(bool reloadAssemblies = false)
        {
            await StopAsync();
            await UnloadAsync(reloadAssemblies);
            await StartAsync();
        }

        public Task StartAsync()
        {
            _currentHost = CreatePluginHost();

            return _currentHost.StartAsync();
        }

        public Task StopAsync()
        {
            return _currentHost?.StopAsync() ?? Task.CompletedTask;
        }

        private async Task UnloadAsync(bool reloadAssemblies = false)
        {
            if (_currentHost is IAsyncDisposable asyncDisposable)
            {
                // Required if plug-ins implement IAsyncDisposable, otherwise an exception will be thrown.
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                _currentHost?.Dispose();
            }

            _currentHost = null;

            if (reloadAssemblies)
            {
                _currentContext?.Unload();
                _currentContext = null;
            }
        }

        private IHost CreatePluginHost()
        {
            var assembliesByName = GetAssemblyInformation();
            _currentContext ??= new PluginAssemblyLoadContext(assembliesByName);
            var pluginHostBuilder = Host.CreateDefaultBuilder();

            Plugins = GetPluginInformation(assembliesByName, _currentContext);

            foreach (var plugin in Plugins)
            {
                plugin.Startup?.ConfigureHost(pluginHostBuilder);
            }

            pluginHostBuilder.ConfigureServices(services =>
            {
                services.AddHostedService<PluginLoaderService>();

                foreach (var (type, lifetime) in _serviceProxyCollection.ServiceTypes)
                {
                    services.Add(new ServiceDescriptor(
                        type,
                        _ => ProxyGenerator.GetServiceProxy(type, _serviceProvider.GetService(type)!),
                        lifetime));
                }

                foreach (var plugin in Plugins)
                {
                    plugin.Startup?.ConfigureServices(services);
                }
            });

            return pluginHostBuilder.Build();
        }

        private List<PluginInformation> GetPluginInformation(
            IDictionary<string, IAssemblyInformation> assembliesByName,
            AssemblyLoadContext context)
        {
            var plugins = new List<PluginInformation>();
            var pluginAssemblies = assembliesByName
                .Values
                .Where(a => a.IsPlugin)
                .Select(a => context.LoadFromAssemblyName(a.AssemblyName))
                .ToList();

            foreach (var assembly in pluginAssemblies)
            {
                // Find plugin startup.
                var pluginStartup = assembly
                    .GetTypes()
                    .Where(t => typeof(IPluginStartup).IsAssignableFrom(t) && t.IsClass)
                    .ToList();

                if (pluginStartup.Count > 1)
                {
                    _logger.LogWarning("A plugin may only define zero or one IPluginStartup implementation ({0}).", assembly);
                    continue;
                }

                // Find plugin.
                var plugin = assembly
                    .GetTypes()
                    .Where(t => typeof(IPlugin).IsAssignableFrom(t)
                                && t.IsClass
                                && !t.IsAbstract
                                && t.GetCustomAttribute<ImpostorPluginAttribute>() != null)
                    .ToList();

                if (plugin.Count != 1)
                {
                    _logger.LogWarning(
                        "A plugin must define exactly one IPlugin or PluginBase implementation ({0}).",
                        assembly);
                    continue;
                }

                // Save plugin.
                plugins.Add(new PluginInformation(
                    pluginStartup
                        .Select(Activator.CreateInstance)
                        .Cast<IPluginStartup>()
                        .FirstOrDefault(),
                    plugin.First()));
            }

            return plugins;
        }

        private IDictionary<string, IAssemblyInformation> GetAssemblyInformation()
        {
            // Add the plugins and libraries.
            var config = _pluginConfig.Value;
            var pluginPaths = new List<string>(config.Paths);
            var libraryPaths = new List<string>(config.LibraryPaths);

            var rootFolder = AppContext.BaseDirectory;

            pluginPaths.Add(Path.Combine(rootFolder, "plugins"));
            libraryPaths.Add(Path.Combine(rootFolder, "libraries"));

            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude("*.dll");
            matcher.AddExclude("Impostor.Api.dll");
            matcher.AddExclude("Impostor.Server.dll");

            var assembliesByName = AssemblyLoadContext.Default.Assemblies
                .Where(a => a.GetName().Name != null)
                .ToDictionary(
                    a => a.GetName().Name!,
                    a => (IAssemblyInformation)new LoadedAssemblyInformation(a, true));

            RegisterAssemblies(assembliesByName, libraryPaths, matcher, false);
            RegisterAssemblies(assembliesByName, pluginPaths, matcher, true);

            return assembliesByName;
        }

        private void RegisterAssemblies(
            IDictionary<string, IAssemblyInformation> assemblies,
            IEnumerable<string> paths,
            Matcher matcher,
            bool isPlugin)
        {
            foreach (var path in paths.SelectMany(matcher.GetResultsInFullPath))
            {
                AssemblyName name;

                try
                {
                    name = AssemblyName.GetAssemblyName(path);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }

                if (name.Name == null)
                {
                    _logger.LogWarning(
                        "Skipped {0} because the assembly name could not be resolved",
                        path);
                    continue;
                }

                if (assemblies.TryGetValue(name.Name, out var loaded))
                {
                    var version = name.Version;
                    var loadedVersion = loaded.AssemblyName.Version;

                    if (loadedVersion != null && version != null && version.Major != loadedVersion.Major)
                    {
                        _logger.LogError(
                            "Assembly {0} is already loaded with version {1} while loading version {2}",
                            name.Name,
                            loadedVersion,
                            version);
                    }
                    else
                    {
                        _logger.LogDebug("Skipped assembly {0} because it's already loaded", name.Name);
                    }

                    continue;
                }

                _logger.LogTrace("Registered assembly {0} v{1} ({3})", name.Name, name.Version, path);

                assemblies.Add(name.Name, new AssemblyInformation(name, path, isPlugin, false));
            }
        }
    }
}
