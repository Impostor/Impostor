using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Manager;
using Impostor.Api.Plugins;
using Impostor.Api.Utils;
using Impostor.Hazel.Extensions;
using Impostor.Server.Events;
using Impostor.Server.Http;
using Impostor.Server.Net;
using Impostor.Server.Net.Custom;
using Impostor.Server.Net.Factories;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Server.Plugins;
using Impostor.Server.Recorder;
using Impostor.Server.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;

namespace Impostor.Server
{
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
            var configuration = CreateConfiguration(args);
            var pluginConfig = configuration.GetSection("PluginLoader")
                .Get<PluginConfig>() ?? new PluginConfig();
            var httpConfig = configuration.GetSection(HttpServerConfig.Section)
                .Get<HttpServerConfig>() ?? new HttpServerConfig();

            var hostBuilder = Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
#if DEBUG
                .UseEnvironment(Environment.GetEnvironmentVariable("IMPOSTOR_ENV") ?? "Development")
#else
                .UseEnvironment("Production")
#endif
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddConfiguration(configuration);
                })
                .ConfigureServices((host, services) =>
                {
                    var debug = host.Configuration
                        .GetSection(DebugConfig.Section)
                        .Get<DebugConfig>() ?? new DebugConfig();

                    services.AddSingleton<ServerEnvironment>();
                    services.AddSingleton<IServerEnvironment>(p => p.GetRequiredService<ServerEnvironment>());
                    services.AddSingleton<IDateTimeProvider, RealDateTimeProvider>();

                    services.Configure<DebugConfig>(host.Configuration.GetSection(DebugConfig.Section));
                    services.Configure<AntiCheatConfig>(host.Configuration.GetSection(AntiCheatConfig.Section));
                    services.Configure<CompatibilityConfig>(host.Configuration.GetSection(CompatibilityConfig.Section));
                    services.Configure<ServerConfig>(host.Configuration.GetSection(ServerConfig.Section));
                    services.Configure<TimeoutConfig>(host.Configuration.GetSection(TimeoutConfig.Section));
                    services.Configure<HttpServerConfig>(host.Configuration.GetSection(HttpServerConfig.Section));

                    services.AddSingleton<ICompatibilityManager, CompatibilityManager>();
                    services.AddSingleton<ClientManager>();
                    services.AddSingleton<IClientManager>(p => p.GetRequiredService<ClientManager>());

                    if (debug.GameRecorderEnabled)
                    {
                        services.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());
                        services.AddSingleton<ObjectPool<PacketSerializationContext>>(serviceProvider =>
                        {
                            var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                            var policy = new PacketSerializationContextPooledObjectPolicy();
                            return provider.Create(policy);
                        });

                        services.AddSingleton<PacketRecorder>();
                        services.AddHostedService(sp => sp.GetRequiredService<PacketRecorder>());
                        services.AddSingleton<IClientFactory, ClientFactory<ClientRecorder>>();
                    }
                    else
                    {
                        services.AddSingleton<IClientFactory, ClientFactory<Client>>();
                    }

                    services.AddSingleton<GameManager>();
                    services.AddSingleton<IGameManager>(p => p.GetRequiredService<GameManager>());
                    services.AddSingleton<ListingManager>();

                    services.AddEventPools();
                    services.AddHazel();
                    services.AddSingleton<ICustomMessageManager<ICustomRootMessage>, CustomMessageManager<ICustomRootMessage>>();
                    services.AddSingleton<ICustomMessageManager<ICustomRpc>, CustomMessageManager<ICustomRpc>>();
                    services.AddSingleton<IMessageWriterProvider, MessageWriterProvider>();
                    services.AddSingleton<IGameCodeFactory, GameCodeFactory>();
                    services.AddSingleton<IEventManager, EventManager>();
                    services.AddSingleton<Matchmaker>();
                    services.AddHostedService<MatchmakerService>();
                })
                .UseSerilog((context, loggerConfiguration) =>
                {
#if DEBUG
                    var logLevel = LogEventLevel.Debug;
#else
                    var logLevel = LogEventLevel.Information;
#endif

                    if (args.Contains("--verbose"))
                    {
                        logLevel = LogEventLevel.Verbose;
                    }
                    else if (args.Contains("--errors-only"))
                    {
                        logLevel = LogEventLevel.Error;
                    }

                    static Assembly? LoadSerilogAssembly(AssemblyLoadContext loadContext, AssemblyName name)
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

                    AssemblyLoadContext.Default.Resolving += LoadSerilogAssembly;

                    loggerConfiguration
                        .MinimumLevel.Is(logLevel)
#if DEBUG
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
#else
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
#endif
                        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .ReadFrom.Configuration(context.Configuration, new ConfigurationReaderOptions(ConfigurationAssemblySource.AlwaysScanDllFiles));

                    AssemblyLoadContext.Default.Resolving -= LoadSerilogAssembly;
                })
                .UseConsoleLifetime()
                .UsePluginLoader(pluginConfig);

            if (httpConfig.Enabled)
            {
                hostBuilder.ConfigureWebHostDefaults(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddControllers();
                    });

                    builder.Configure(app =>
                    {
                        var pluginLoaderService = app.ApplicationServices.GetRequiredService<PluginLoaderService>();
                        foreach (var pluginInformation in pluginLoaderService.Plugins)
                        {
                            if (pluginInformation.Startup is IPluginHttpStartup httpStartup)
                            {
                                httpStartup.ConfigureWebApplication(app);
                            }
                        }

                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });

                    builder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Listen(IPAddress.Parse(httpConfig.ListenIp), httpConfig.ListenPort);
                    });
                });
            }

            return hostBuilder;
        }
    }
}
