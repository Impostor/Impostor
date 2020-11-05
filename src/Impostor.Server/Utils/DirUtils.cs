using System;
using System.Diagnostics;
using System.IO;

namespace Impostor.Server.Utils
{
    internal static class DirUtils
    {
        // Workaround for:
        // - https://github.com/dotnet/runtime/issues/40828
        // - https://github.com/dotnet/runtime/issues/38405#issuecomment-652643506
        public static string GetExecutableDirectory()
        {
            var programDirectory = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
            var currentDirectory = Directory.GetCurrentDirectory();
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var executablePath = Process.GetCurrentProcess().MainModule!.FileName;
            var executableDirectory = Path.GetDirectoryName(executablePath)!;
            var executable = Path.GetFileNameWithoutExtension(executablePath);

            if ("dotnet".Equals(executable, StringComparison.InvariantCultureIgnoreCase))
            {
                return programDirectory;
            }

            return executableDirectory;
        }
    }
}
