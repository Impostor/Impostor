using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Impostor.Server.Plugins;

public class AssemblyInformation(AssemblyName assemblyName, string path, bool isPlugin) : IAssemblyInformation
{
    private Assembly? _assembly;

    public string Path { get; } = path;

    public bool IsPlugin { get; } = isPlugin;

    public AssemblyName AssemblyName { get; } = assemblyName;

    public Assembly Load(AssemblyLoadContext context)
    {
        if (_assembly != null)
        {
            return _assembly;
        }

        using var stream = File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read);

        _assembly = context.LoadFromStream(stream);

        return _assembly;
    }
}
