using System;
using Impostor.Server.Data;
using Impostor.Server.Net;
using Impostor.Server.Net.Manager;
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
#else
                .MinimumLevel.Information()
                
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
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
                    builder.AddJsonFile("config.json", false);
                    builder.AddEnvironmentVariables(prefix: "IMPOSTOR_");
                    builder.AddCommandLine(args);
                })
                .ConfigureServices((host, services) =>
                {
                    services.Configure<ServerConfig>(host.Configuration.GetSection("Server"));

                    services.AddSingleton<GameManager>();
                    services.AddSingleton<Matchmaker>();
                    
                    services.AddHostedService<MatchmakerService>();
                })
                .UseSerilog();
    }
}