using System;
using Impostor.Server.Data;
using Impostor.Server.Net;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Redirector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Impostor.Server
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Verbose()
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
                        services.AddSingleton<INodeProvider, NodeProviderRedis>();
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = redirector.Redis;
                            options.InstanceName = "ImpostorRedis";
                        });
                    }
                    else
                    {
                        services.AddSingleton<INodeProvider, NodeProviderNoOp>();
                    }
                    
                    if (redirector.Enabled && redirector.Master)
                    {
                        services.AddSingleton<IClientManager, ClientManagerRedirector>();
                    }
                    else
                    {
                        services.AddSingleton<IClientManager, ClientManager>();
                        services.AddSingleton<GameManager>();
                    }
                    
                    services.AddSingleton<Matchmaker>();
                    
                    services.AddHostedService<MatchmakerService>();
                })
                .UseSerilog();
    }
}