using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Impostor.Api.Plugins;

namespace Impostor.Server.Plugins.Informations;

public class PluginInformation
{
    private readonly ImpostorPluginAttribute _attribute;

    public PluginInformation(IPluginStartup? startup, Type pluginType, Assembly assembly)
    {
        _attribute = pluginType.GetCustomAttribute<ImpostorPluginAttribute>()!;
        
        Name = _attribute.Name ??
               assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? assembly.GetName().Name!;
        Author = _attribute.Author ?? assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;
        Version = _attribute.Version ??
                  assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
                  assembly.GetName().Version!.ToString();

        Dependencies = pluginType.GetCustomAttributes<ImpostorDependencyAttribute>()
            .Select(t => new DependencyInformation(t)).ToList();
        Startup = startup;
        PluginType = pluginType;
        Assembly = assembly;
    }

    public string Id
    {
        get => _attribute.Id;
    }
    
    public Assembly Assembly { get; internal set; }

    public string Name { get; }

    public string Author { get; }

    public string Version { get; }

    public List<DependencyInformation> Dependencies { get; }

    public IPluginStartup? Startup { get; }

    public Type PluginType { get; }

    public IPlugin? Instance { get; set; }
    
    public bool AssemblyPart { get; internal set; }

    public override string ToString()
    {
        return $"{Id} {Name} ({Version}) by {Author}";
    }
}
