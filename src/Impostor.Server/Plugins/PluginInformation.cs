using System;
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

            Startup = startup;
            PluginType = pluginType;
        }

        public string Package => _attribute.Package;

        public string Name => _attribute.Name;

        public string Author => _attribute.Author;

        public string Version => _attribute.Version;

        public IPluginStartup? Startup { get; }

        public Type PluginType { get; }

        public IPlugin? Instance { get; set; }

        public override string ToString()
        {
            return $"{Package} {Name} ({Version}) by {Author}";
        }
    }
}
