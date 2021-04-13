using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Impostor.Api.Plugins;

namespace Impostor.Server.Plugins
{
    public class PluginInformation
    {
        private readonly ImpostorPluginAttribute _attribute;

        public PluginInformation(IPluginStartup? startup, Type pluginType)
        {
            _attribute = pluginType.GetCustomAttribute<ImpostorPluginAttribute>()!;

            Dependencies = pluginType.GetCustomAttributes<ImpostorDependencyAttribute>().Select(t => new DependencyInformation(t)).ToList();
            Startup = startup;
            PluginType = pluginType;
        }

        public string Id => _attribute.Id;

        public string Name => _attribute.Name;

        public string Author => _attribute.Author;

        public string Version => _attribute.Version;

        public List<DependencyInformation> Dependencies { get; }

        public IPluginStartup? Startup { get; }

        public Type PluginType { get; }

        public IPlugin? Instance { get; set; }

        public override string ToString()
        {
            return $"{Id} {Name} ({Version}) by {Author}";
        }
    }
}
