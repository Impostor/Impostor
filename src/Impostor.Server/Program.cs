using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Manager;
using Impostor.Api.Utils;
using Impostor.Server.Events;
using Impostor.Server.Net;
using Impostor.Server.Net.Custom;
using Impostor.Server.Net.Factories;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Server.Plugins;
using Impostor.Server.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Next.Hazel.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;
using PluginConfig = Impostor.Api.Config.PluginConfig;

namespace Impostor.Server;

internal static class Program
{
    private static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting Impostor v{0}", DotnetUtils.Version);
            CreateHostBuilder(args).Build().Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Impostor terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IConfiguration CreateConfiguration(string[] args)
    {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile("config.json", true);
        configurationBuilder.AddJsonFile("config.Development.json", true);
        configurationBuilder.AddEnvironmentVariables(prefix: "IMPOSTOR_");
        configurationBuilder.AddCommandLine(args);

        return configurationBuilder.Build();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var configuration = CreateConfiguration(args)
            .GetConfig<ServerConfig>(ServerConfig.Section, out var serverConfig)
            .GetConfig<PluginConfig>(PluginConfig.Section, out var pluginConfig);

        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureServer(configuration)
            .ConfigureLog(serverConfig.LogLevel)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseEnvironment(serverConfig.Env ?? DotnetUtils.Environment)
            .UseConsoleLifetime()
            .UsePluginLoader(pluginConfig);

        return hostBuilder;
    }

    private static IHostBuilder ConfigureServer(this IHostBuilder builder, IConfiguration configuration)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddConfiguration(configuration);
            })
            .ConfigureServices((host, services) =>
            {
                services.AddSingleton<ServerEnvironment>();
                services.AddSingleton<IServerEnvironment>(p => p.GetRequiredService<ServerEnvironment>());
                services.AddSingleton<IDateTimeProvider, RealDateTimeProvider>();

                services.Configure<AntiCheatConfig>(host.Configuration.GetSection(AntiCheatConfig.Section));
                services.Configure<CompatibilityConfig>(host.Configuration.GetSection(CompatibilityConfig.Section));
                services.Configure<ServerConfig>(host.Configuration.GetSection(ServerConfig.Section));
                services.Configure<TimeoutConfig>(host.Configuration.GetSection(TimeoutConfig.Section));

                services.AddSingleton<ICompatibilityManager, CompatibilityManager>();
                services.AddSingleton<ClientManager>();
                services.AddSingleton<IClientManager>(p => p.GetRequiredService<ClientManager>());

                services.AddSingleton<IClientFactory, ClientFactory<Client>>();
                services.AddSingleton<GameManager>();
                services.AddSingleton<IGameManager>(p => p.GetRequiredService<GameManager>());

                services.AddEventPools();
                services.AddHazel();
                services.AddSingleton<ICustomMessageManager<ICustomRootMessage>, CustomMessageManager<ICustomRootMessage>>();
                services.AddSingleton<ICustomMessageManager<ICustomRpc>, CustomMessageManager<ICustomRpc>>();
                services.AddSingleton<IMessageWriterProvider, MessageWriterProvider>();
                services.AddSingleton<IGameCodeFactory, GameCodeFactory>();
                services.AddSingleton<IEventManager, EventManager>();
                services.AddSingleton<NetListenerManager>();
                services.AddHostedService<NetApiService>();
                services.AddHostedService<StarterService>();
            });
        return builder;
    }

    private static IHostBuilder ConfigureLog(this IHostBuilder hostBuilder, LogEventLevel minimumLevel)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
        {
            AssemblyLoadContext.Default.Resolving += LoadSerilogAssembly;

            loggerConfiguration
                .MinimumLevel.Is(minimumLevel)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions(ConfigurationAssemblySource.AlwaysScanDllFiles));

            AssemblyLoadContext.Default.Resolving -= LoadSerilogAssembly;
        });
        return hostBuilder;

        Assembly? LoadSerilogAssembly(AssemblyLoadContext loadContext, AssemblyName name)
        {
            var paths = new[] { AppDomain.CurrentDomain.BaseDirectory, Directory.GetCurrentDirectory() };
            foreach (var path in paths)
            {
                try
                {
                    return loadContext.LoadFromAssemblyPath(Path.Combine(path, name.Name + ".dll"));
                }
                catch (FileNotFoundException)
                {
                }
            }

            return null;
        }
    }

    private static IHostBuilder ConfigureExtension(this IHostBuilder builder)
    {
        return builder;
    }

    private static IConfiguration GetConfig<T>(this IConfiguration configuration, string section, out T result) where T : class, new()
    {
        result = configuration.GetSection(section)
            .Get<T>() ?? new T();
        return configuration;
    }
}
