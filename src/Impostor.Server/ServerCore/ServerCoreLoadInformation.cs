using System.Reflection;

namespace Impostor.Server.ServerCore;

public class ServerCoreLoadInformation(AssemblyName assemblyName, string path)
{
    public readonly AssemblyName AssemblyName = assemblyName;

    public readonly string Path = path;
}
