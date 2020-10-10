using System;
using Impostor.Server.Data;
using Impostor.Server.Events;
using Impostor.Server.Events.Managers;
using Impostor.Server.Games.Managers;
using Impostor.Server.Hazel;
using Impostor.Server.Net;
using Impostor.Server.Net.Factories;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Redirector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Impostor.Server
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
#else
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
#endif
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting Impostor");
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

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
#if DEBUG
                .UseEnvironment(Environment.GetEnvironmentVariable("IMPOSTOR_ENV") ?? "Development")
#else
                .UseEnvironment("Production")
#endif
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("config.json", true);
                    builder.AddJsonFile("config.Development.json", true);
                    builder.AddEnvironmentVariables(prefix: "IMPOSTOR_");
                    builder.AddCommandLine(args);
                })
                .ConfigureServices((host, services) =>
                {
                    var redirector = host.Configuration
                        .GetSection(ServerRedirectorConfig.Section)
                        .Get<ServerRedirectorConfig>() ?? new ServerRedirectorConfig();

                    services.Configure<ServerConfig>(host.Configuration.GetSection(ServerConfig.Section));
                    services.Configure<ServerRedirectorConfig>(host.Configuration.GetSection(ServerRedirectorConfig.Section));

                    if (redirector.Enabled)
                    {
                        if (!string.IsNullOrEmpty(redirector.Locator.Redis))
                        {
                            // When joining a game, it retrieves the game server ip from redis.
                            // When a game has been created on this node, it stores the game code with its ip in redis.
                            services.AddSingleton<INodeLocator, NodeLocatorRedis>();

                            // Dependency for the NodeLocatorRedis.
                            services.AddStackExchangeRedisCache(options =>
                            {
                                options.Configuration = redirector.Locator.Redis;
                                options.InstanceName = "ImpostorRedis";
                            });
                        }
                        else if (!string.IsNullOrEmpty(redirector.Locator.UdpMasterEndpoint))
                        {
                            services.AddSingleton<INodeLocator, NodeLocatorUdp>();

                            if (redirector.Master)
                            {
                                services.AddHostedService<NodeLocatorUdpService>();
                            }
                        }
                        else
                        {
                            throw new Exception("Missing a valid NodeLocator config.");
                        }

                        // Use the configuration as source for the list of nodes to provide
                        // when creating a game.
                        services.AddSingleton<INodeProvider, NodeProviderConfig>();
                    }
                    else
                    {
                        // Redirector is not enabled but the dependency is still required.
                        // So we provide one that ignores all calls.
                        services.AddSingleton<INodeLocator, NodeLocatorNoOp>();
                    }

                    services.AddSingleton<IClientManager, ClientManager>();

                    if (redirector.Enabled && redirector.Master)
                    {
                        services.AddSingleton<IClientFactory, ClientFactory<ClientRedirector>>();

                        // For a master server, we don't need a GameManager.
                    }
                    else
                    {
                        services.AddSingleton<IClientFactory, ClientFactory<Client>>();
                        services.AddSingleton<IGameManager, GameManager>();
                    }

                    services.AddSingleton<IEventManager, EventManager>();
                    services.UseHazelMatchmaking();
                    services.AddHostedService<MatchmakerService>();
                })
                .UseConsoleLifetime()
                .UseSerilog();
    }
}