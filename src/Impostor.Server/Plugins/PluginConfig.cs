using System.Collections.Generic;

namespace Impostor.Server.Plugins;

public class PluginConfig
{
    public List<string> Paths { get; set; } = new();

    public List<string> LibraryPaths { get; set; } = new();
}
