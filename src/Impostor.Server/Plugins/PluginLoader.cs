using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Impostor.Api.Config;
using Impostor.Api.Extension;
using Impostor.Api.Plugins;
using Impostor.Server.Plugins.Informations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Impostor.Server.Plugins;

internal static class PluginLoader
{
    private static readonly ILogger Logger = Log.ForContext(typeof(PluginLoader));

    private static bool IsTargetType(Type type)
    {
        if (!type.IsClass)
        {
            return false;
        }

        if (type.IsAbstract)
        {
            return false;
        }

        if (typeof(IPlugin).IsAssignableFrom(type))
        {
            return true;
        }

        if (typeof(IPluginStartup).IsAssignableFrom(type))
        {
            return true;
        }

        return false;
    }

    public static IHostBuilder UsePluginLoader(this IHostBuilder builder, PluginConfig config)
    {
        var assemblyInfos = new List<IAssemblyInformation>();
        var context = AssemblyLoadContext.Default;

        // Add the plugins and libraries.
        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude("*.dll");
        matcher.AddExclude("Impostor.Api.dll");

        config
            .PluginPaths
            .CheckPaths()
            .RegisterAssemblies(matcher, assemblyInfos, true);

        config
            .LibraryPaths
            .CheckPaths()
            .RegisterAssemblies(matcher, assemblyInfos, false);

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
        var cacher = new TypesCacher(IsTargetType);

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
                            && t is { IsClass: true, IsAbstract: false }
                            && t.GetCustomAttribute<ImpostorPluginAttribute>() != null)
                .ToList();

            if (plugin.Count != 1)
            {
                Logger.Warning("A plugin must define exactly one IPlugin or PluginBase implementation ({0}).",
                    assembly);
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
            services.AddSingleton<PluginLoaderService>(provider =>
                ActivatorUtilities.CreateInstance<PluginLoaderService>(provider, orderedPlugins));
            services.AddSingleton<IHostedService>(p => p.GetRequiredService<PluginLoaderService>());

            foreach (var plugin in orderedPlugins)
            {
                plugin.Startup?.ConfigureServices(services);
            }
        });

        return builder;
    }

    public static void ConfigurePluginWeb(this IApplicationBuilder app, IWebHostBuilder webHostBuilder)
    {
        var plugins = app.ApplicationServices.GetRequiredService<PluginLoaderService>().Plugins;
        var mvcBuilder = app.ApplicationServices.GetRequiredService<IMvcBuilder>();
        foreach (var pluginInfo in plugins)
        {
            if (pluginInfo.Startup is IHttpPluginStartup startup)
            {
                startup.ConfigureHost(webHostBuilder);
                startup.ConfigureWebApplication(app);
            }

            if (pluginInfo.Instance is IHttpPlugin { AssemblyPart: true })
            {
                mvcBuilder.AddApplicationPart(pluginInfo.PluginType.Assembly);
            }
        }
    }

    private static List<PathCheckInfo> CheckPaths(this IEnumerable<string> paths)
    {
        var infos = new List<PathCheckInfo>();
        foreach (var path in paths)
        {
            if (path is "plugins" or "libraries")
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                infos.Add(new PathCheckInfo(dir, true));
                continue;
            }

            if (Directory.Exists(path))
            {
                infos.Add(new PathCheckInfo(path, true));
                continue;
            }

            if (File.Exists(path))
            {
                infos.Add(new PathCheckInfo(path, false));
                continue;
            }

            Logger.Warning("PluginLoader Check Path: {0} does not exist.", path);
        }

        return infos;
    }

    private static void RegisterAssemblies(
        this List<PathCheckInfo> pathInfos,
        Matcher matcher,
        ICollection<IAssemblyInformation> assemblyInfos,
        bool isPlugin)
    {
        var files = pathInfos.SelectMany(n => n.IsDir ? matcher.GetResultsInFullPath(n.Path) : [n.Path]);
        foreach (var path in files)
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
        foreach (var plugin in checkedPlugins.Where(plugin => !processed.Contains(plugin)))
        {
            RecursiveOrder(plugin, dependencyGraph, processed, ordered, pluginDictionary);
        }

        return ordered;
    }

    private static List<string> CheckHardDependencies(
        List<string> plugins,
        IReadOnlyDictionary<string, List<string>> hardDependencies)
    {
        foreach (var plugin in plugins)
        {
            if (!hardDependencies.TryGetValue(plugin, out var hardDependency))
            {
                continue;
            }

            foreach (var dependency in hardDependency.Where(dependency => !plugins.Contains(dependency)))
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

        foreach (var dependency in dependencyGraph[plugin].Where(dependency => !processed.Contains(dependency)))
        {
            RecursiveOrder(dependency, dependencyGraph, processed, ordered, pluginDictionary);
        }

        ordered.Add(pluginDictionary[plugin]);
    }

    private record PathCheckInfo(string Path, bool IsDir);

    private class TypesCacher(Predicate<Type> predicate)
    {
        private List<Type> _types = [];

        internal TypesCacher LoaderAssembly(Assembly assembly)
        {
            return this;
        }
    }
}
