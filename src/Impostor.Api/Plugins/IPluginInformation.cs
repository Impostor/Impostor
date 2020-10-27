using System;
using Impostor.Api.Plugins;

namespace Impostor.Server.Plugins
{
    public interface IPluginInformation
    {
        string Package { get; }

        string Name { get; }

        string Author { get; }

        string Version { get; }

        IPluginStartup? Startup { get; }

        IPlugin? Instance { get; }

        Type PluginType { get; }
    }
}
