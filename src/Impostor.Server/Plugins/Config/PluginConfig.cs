using System.Collections.Generic;

namespace Impostor.Server.Plugins.Config
{
    public class PluginConfig
    {
        public const string Section = "PluginLoader";

        public List<string> Paths { get; set; } = new List<string>();

        public List<string> LibraryPaths { get; set; } = new List<string>();
    }
}
