using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Impostor.Server.Plugins
{
    public static class PluginLoader
    {
        private static readonly ILogger Logger = Log.ForContext(typeof(PluginLoader));

        public static IHostBuilder UsePluginLoader(this IHostBuilder builder, PluginConfig config)
        {
            var assemblyInfos = new List<IAssemblyInformation>();
            var context = AssemblyLoadContext.Default;

            // Add the plugins and libraries.
            var pluginPaths = new List<string>(config.Paths);
            var libraryPaths = new List<string>(config.LibraryPaths);
            CheckPaths(pluginPaths);
            CheckPaths(libraryPaths);

            var rootFolder = Directory.GetCurrentDirectory();

            pluginPaths.Add(Path.Combine(rootFolder, "plugins"));
            libraryPaths.Add(Path.Combine(rootFolder, "libraries"));

            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude("*.dll");
            matcher.AddExclude("Impostor.Api.dll");

            RegisterAssemblies(pluginPaths, matcher, assemblyInfos, true);
            RegisterAssemblies(libraryPaths, matcher, assemblyInfos, false);

            // Register the resolver to the current context.
            // TODO: Move this to a new context so we can unload/reload plugins.
            context.Resolving += (loadContext, name) =>
            {
                Logger.Verbose("Loading assembly {0} v{1}", name.Name, name.Version);

                // Some plugins may be referencing another Impostor.Api version and try to load it.
                // We want to only use the one shipped with the server.
                if (name.Name == "Impostor.Api")
                {
                    return typeof(IPlugin).Assembly;
                }

                var info = assemblyInfos.FirstOrDefault(a => a.AssemblyName.Name == name.Name);

                return info?.Load(loadContext);
            };

            // TODO: Catch uncaught exceptions.
            var assemblies = assemblyInfos
                .Where(a => a.IsPlugin)
                .Select(a => context.LoadFromAssemblyName(a.AssemblyName))
                .ToList();

            // Find all plugins.
            var plugins = new List<PluginInformation>();

            foreach (var assembly in assemblies)
            {
                // Find plugin startup.
                var pluginStartup = assembly
                    .GetTypes()
                    .Where(t => typeof(IPluginStartup).IsAssignableFrom(t) && t.IsClass)
                    .ToList();

                if (pluginStartup.Count > 1)
                {
                    Logger.Warning("A plugin may only define zero or one IPluginStartup implementation ({0}).", assembly);
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
                    Logger.Warning("A plugin must define exactly one IPlugin or PluginBase implementation ({0}).", assembly);
                    continue;
                }

                // Save plugin.
                plugins.Add(new PluginInformation(
                    pluginStartup
                        .Select(Activator.CreateInstance)
                        .Cast<IPluginStartup>()
                        .FirstOrDefault(),
                    plugin.Single()));
            }

            var orderedPlugins = LoadOrderPlugins(plugins);

            foreach (var plugin in orderedPlugins)
            {
                plugin.Startup?.ConfigureHost(builder);
            }

            builder.ConfigureServices(services =>
            {
                services.AddHostedService(provider => ActivatorUtilities.CreateInstance<PluginLoaderService>(provider, orderedPlugins));

                foreach (var plugin in orderedPlugins)
                {
                    plugin.Startup?.ConfigureServices(services);
                }
            });

            return builder;
        }

        private static void CheckPaths(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                if (!Directory.Exists(path))
                {
                    Logger.Warning("Path {path} was specified in the PluginLoader configuration, but this folder doesn't exist!", path);
                }
            }
        }

        private static void RegisterAssemblies(
            IEnumerable<string> paths,
            Matcher matcher,
            ICollection<IAssemblyInformation> assemblyInfos,
            bool isPlugin)
        {
            foreach (var path in paths.SelectMany(matcher.GetResultsInFullPath))
            {
                AssemblyName assemblyName;

                try
                {
                    assemblyName = AssemblyName.GetAssemblyName(path);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }

                assemblyInfos.Add(new AssemblyInformation(assemblyName, path, isPlugin));
            }
        }

        private static List<PluginInformation> LoadOrderPlugins(IEnumerable<PluginInformation> plugins)
        {
            var pluginDictionary = new Dictionary<string, PluginInformation>();
            var hardDependencies = new Dictionary<string, List<string>>();

            foreach (var plugin in plugins)
            {
                pluginDictionary[plugin.Id] = plugin;
                hardDependencies[plugin.Id] = plugin
                    .Dependencies
                    .Where(p => p.DependencyType == DependencyType.HardDependency)
                    .Select(p => p.Id)
                    .ToList();
            }

            var presentPlugins = pluginDictionary.Keys.ToList();

            // Check whether the Hard Dependencies are present and remove those without.
            var checkedPlugins = CheckHardDependencies(presentPlugins, hardDependencies);

            var dependencyGraph = checkedPlugins.ToDictionary(p => p, _ => new List<string>());

            foreach (var plugin in checkedPlugins)
            {
                foreach (var dependency in pluginDictionary[plugin].Dependencies.Where(d => checkedPlugins.Contains(d.Id)))
                {
                    if (dependency.DependencyType == DependencyType.LoadBefore)
                    {
                        dependencyGraph[dependency.Id].Add(plugin);
                    }
                    else
                    {
                        dependencyGraph[plugin].Add(dependency.Id);
                    }
                }
            }

            var processed = new List<string>();
            var ordered = new List<PluginInformation>();
            foreach (var plugin in checkedPlugins)
            {
                if (!processed.Contains(plugin))
                {
                    RecursiveOrder(plugin, dependencyGraph, processed, ordered, pluginDictionary);
                }
            }

            return ordered;
        }

        private static List<string> CheckHardDependencies(
            List<string> plugins,
            IReadOnlyDictionary<string, List<string>> hardDependencies)
        {
            foreach (var plugin in plugins)
            {
                if (!hardDependencies.ContainsKey(plugin))
                {
                    continue;
                }

                foreach (var dependency in hardDependencies[plugin].Where(dependency => !plugins.Contains(dependency)))
                {
                    Logger.Error(
                        "The plugin {plugin} has defined the plugin {dependency} as a hard dependency but its not present! {plugin} will not loaded.",
                        plugin,
                        dependency,
                        plugin
                    );

                    // Remove the plugin from the plugins to load.
                    plugins.Remove(plugin);

                    // Since other plugins might have defined the removed plugin as a hard dependency a recheck is necessary.
                    return CheckHardDependencies(plugins, hardDependencies);
                }
            }

            return plugins;
        }

        private static void RecursiveOrder(
            string plugin,
            IReadOnlyDictionary<string, List<string>> dependencyGraph,
            ICollection<string> processed,
            ICollection<PluginInformation> ordered,
            IReadOnlyDictionary<string, PluginInformation> pluginDictionary)
        {
            processed.Add(plugin);

            foreach (var dependency in dependencyGraph[plugin])
            {
                // First add the dependencies using a recursive call before adding itself.
                if (!processed.Contains(dependency))
                {
                    RecursiveOrder(dependency, dependencyGraph, processed, ordered, pluginDictionary);
                }
            }

            ordered.Add(pluginDictionary[plugin]);
        }
    }
}
