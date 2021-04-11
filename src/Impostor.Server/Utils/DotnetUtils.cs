using System.Reflection;

namespace Impostor.Server.Utils
{
    public static class DotnetUtils
    {
        private static string? _version;

        public static string Version
        {
            get
            {
                if (_version == null)
                {
                    var attribute = typeof(DotnetUtils).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                    _version = attribute != null ? attribute.InformationalVersion : "UNKNOWN";
                }

                return _version;
            }
        }
    }
}
