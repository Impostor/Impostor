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
                    plugin.First()));
            }

            var orderedPlugins = OrderPlugins(plugins);

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

        private static IEnumerable<PluginInformation> OrderPlugins(IEnumerable<PluginInformation> plugins)
        {
            var pluginsDict = new Dictionary<string, PluginInformation>();
            var dependencyGraph = new Dictionary<string, List<string>>();

            // Create a dependency graph and a dictionary to simplify
            foreach (var plugin in plugins)
            {
                pluginsDict.Add(plugin.Name, plugin);
                dependencyGraph.Add(plugin.Name, new List<string>());
            }

            var pluginList = dependencyGraph.Keys.ToList();

            // Add the dependencies to the graph
            foreach (var plugin in plugins)
            {
                // Add the hard dependencies, iff one is missing give an error message.
                foreach (var hardDepend in plugin.Dependencies)
                {
                    if (pluginList.Contains(hardDepend))
                    {
                        dependencyGraph[plugin.Name].Add(hardDepend);
                    }
                    else
                    {
                        Logger.Error($"The plugin {plugin.Name} has defined the plugin {hardDepend}" +
                                     $" as a hard dependency but {hardDepend} is not present!");
                    }
                }

                foreach (var softDepend in plugin.SoftDependencies)
                {
                    if (pluginList.Contains(softDepend))
                    {
                        dependencyGraph[plugin.Name].Add(softDepend);
                    }
                }

                foreach (var loadBefore in plugin.LoadBefore)
                {
                    if (pluginList.Contains(loadBefore))
                    {
                        dependencyGraph[loadBefore].Add(plugin.Name);
                    }
                }
            }

            var processed = new List<string>();
            var ordered = new List<PluginInformation>();
            foreach (var plugin in pluginList)
            {
                if (!processed.Contains(plugin))
                {
                    RecursiveOrder(plugin, dependencyGraph, processed, ordered, pluginsDict);
                }
            }

            return ordered;
        }

        private static void RecursiveOrder(
            string plugin,
            IReadOnlyDictionary<string, List<string>> dependencyGraph,
            ICollection<string> processed,
            ICollection<PluginInformation> ordered,
            IReadOnlyDictionary<string, PluginInformation> pluginDict)
        {
            processed.Add(plugin);

            foreach (var dep in dependencyGraph[plugin])
            {
                // First add the dependencies using a recursive call before adding itself.
                if (!processed.Contains(dep))
                {
                    RecursiveOrder(dep, dependencyGraph, processed, ordered, pluginDict);
                }
            }

            ordered.Add(pluginDict[plugin]);
        }
    }
}
