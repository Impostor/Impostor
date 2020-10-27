using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Impostor.Server.Plugins
{
    public class PluginAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly IDictionary<string, IAssemblyInformation> _assembliesByName;

        public PluginAssemblyLoadContext(IDictionary<string, IAssemblyInformation> assembliesByName)
            : base(isCollectible: true)
        {
            _assembliesByName = assembliesByName;
        }

        protected override Assembly? Load(AssemblyName name)
        {
            return name.Name != null && _assembliesByName.TryGetValue(name.Name, out var assemblyInfo)
                ? assemblyInfo.Load(this)
                : null;
        }
    }
}
