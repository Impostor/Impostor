using System.Reflection;

namespace Impostor.Server.Utils
{
    internal static class DotnetUtils
    {
        private const string DefaultUnknownBuild = "UNKNOWN";

        public static string GetVersion()
        {
            var attribute = typeof(DotnetUtils).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attribute != null)
            {
                return attribute.InformationalVersion;
            }

            return DefaultUnknownBuild;
        }
    }
}
