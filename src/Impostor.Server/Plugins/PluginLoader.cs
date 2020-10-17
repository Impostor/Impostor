using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;

namespace Impostor.Server.Plugins
{
    public static class PluginLoader
    {
        public static IHostBuilder UsePluginLoader(this IHostBuilder builder, PluginConfig config)
        {
            var assemblyInfos = new List<IAssemblyInformation>();
            var context = AssemblyLoadContext.Default;

            // Add the plugins and libraries.
            var pluginPaths = new List<string>(config.Paths);
            var libraryPaths = new List<string>(config.LibraryPaths);

            var rootFolder = Assembly.GetEntryAssembly()?.Location;
            if (rootFolder != null)
            {
                pluginPaths.Add(Path.Combine(rootFolder, "plugins"));
                libraryPaths.Add(Path.Combine(rootFolder, "libraries"));
            }

            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            matcher.AddInclude("*.dll");
            matcher.AddExclude("Impostor.Server.Api.dll");
            matcher.AddExclude("Impostor.Shared.dll");

            RegisterAssemblies(pluginPaths, matcher, assemblyInfos, true);
            RegisterAssemblies(libraryPaths, matcher, assemblyInfos, false);

            // Register the resolver to the current context.
            // TODO: Move this to a new context so we can unload/reload plugins.
            context.Resolving += (loadContext, name) =>
            {
                var info = assemblyInfos.FirstOrDefault(a => a.AssemblyName.Name == name.Name);

                return info?.Load(loadContext);
            };

            // TODO: Catch uncaught exceptions.
            var assemblies = assemblyInfos
                .Where(a => a.IsPlugin)
                .Select(a => context.LoadFromAssemblyName(a.AssemblyName))
                .ToList();

            var plugins = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(typeof(IPlugin).IsAssignableFrom)
                .Select(Activator.CreateInstance)
                .Cast<IPlugin>()
                .ToList();

            foreach (var plugin in plugins)
            {
                plugin.ConfigureHost(builder);
            }

            builder.ConfigureServices(services =>
            {
                foreach (var plugin in plugins)
                {
                    plugin.ConfigureServices(services);
                }
            });

            return builder;
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
    }
}