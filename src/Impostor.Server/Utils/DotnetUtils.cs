using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Server.Utils;

public static class DotnetUtils
{
    private static string? _version;

    public static string Version
    {
        get
        {
            if (_version != null)
            {
                return _version;
            }

            var attribute = typeof(DotnetUtils).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            _version = attribute != null ? attribute.InformationalVersion : "UNKNOWN";

            return _version;
        }
    }

#pragma warning disable CS0162
    public static bool IsDev
    {
        get
        {
#if DEBUG
            return true;
#endif
            return false;
        }
    }
#pragma warning restore CS0162

    public static string Environment
    {
        get => IsDev ? Environments.Development : Environments.Production;
    }
}
