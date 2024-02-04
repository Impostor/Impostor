using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Hosting;

namespace Impostor.Server.ServerCore;

public static class ServerCoreLoader
{
    private static readonly List<ServerCoreLoadInformation> ServerCoreLoadInfos = [];

    public static IHostBuilder UseServerCoreLoader(this IHostBuilder builder)
    {
        var rootFolder = Directory.GetCurrentDirectory();
        var serverCoreFolder = Path.Combine(rootFolder, "Core");

        if (!Directory.Exists(serverCoreFolder))
        {
            Directory.CreateDirectory(serverCoreFolder);
        }

        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude("*.dll");
        matcher.AddExclude("Impostor.Api.dll");

        foreach (var path in matcher.GetResultsInFullPath(serverCoreFolder))
        {
            AssemblyName assemblyName;

            try
            {
                assemblyName = AssemblyName.GetAssemblyName(path);
            }
            catch (BadImageFormatException)
            {
                continue;
            }

            ServerCoreLoadInfos.Add(new ServerCoreLoadInformation(assemblyName, path));
        }

        return builder;
    }
}
