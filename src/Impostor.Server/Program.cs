using System;
using System.Linq;
using Agones;
using Grpc.Core;
using Impostor.Server.Data;
using Impostor.Server.Net;
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
                        .Get<ServerRedirectorConfig>();
                    services.Configure<ServerConfig>(host.Configuration.GetSection(ServerConfig.Section));
                    services.Configure<ServerRedirectorConfig>(host.Configuration.GetSection(ServerRedirectorConfig.Section));

                    if (redirector.Enabled)
                    {
                        // When joining a game, it retrieves the game server ip from redis.
                        // When a game has been created on this node, it stores the game code with its ip in redis.
                        services.AddSingleton<INodeLocator, NodeLocatorRedis>();
                        
                        // Use the configuration as source for the list of nodes to provide
                        // when creating a game.
                        services.AddSingleton<INodeProvider, NodeProviderAgones>();
                        Console.WriteLine(redirector.Redis);
                        // Dependency for the NodeLocatorRedis.
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = redirector.Redis;
                            options.InstanceName = "ImpostorRedis";
                        });
                    }
                    else
                    {
                        // Redirector is not enabled but the dependency is still required.
                        // So we provide one that ignores all calls.
                        services.AddSingleton<INodeLocator, NodeLocatorNoOp>();
                    }
                    var agones = new AgonesSDK();

                    if (redirector.Enabled && redirector.Master)
                    {
                        services.AddSingleton<IClientManager, ClientManagerRedirector>();
                        // For a master server, we don't need a GameManager.
                    }
                    else
                    {
                        services.AddSingleton<IClientManager, ClientManager>();
                        services.AddSingleton<GameManager>();
                        bool ok = agones.ConnectAsync().Result;
                        if (!ok)
                        {
                            Log.Fatal("failed to setup agones");
                        }
                        var gameServer = agones.GetGameServerAsync().Result;
                        services.Configure<ServerConfig>(myOptions =>
                        {
                            myOptions.PublicIp = gameServer.Status.Address;
                            myOptions.PublicPort = Convert.ToUInt16(gameServer.Status.Ports.First().Port_);
                        });
                        services.AddHostedService<AgonesHealthCheck>();
                    }
                    services.AddSingleton(agones);
                    services.AddSingleton<Matchmaker>();
                    services.AddHostedService<MatchmakerService>();
                })
                .UseSerilog();
    }
}