using System.Reflection;
using System.Runtime.Loader;

namespace Impostor.Server.Plugins;

public class LoadedAssemblyInformation(Assembly assembly) : IAssemblyInformation
{
    public AssemblyName AssemblyName { get; } = assembly.GetName();

    public bool IsPlugin => false;

    public Assembly Load(AssemblyLoadContext context)
    {
        return assembly;
    }
}
