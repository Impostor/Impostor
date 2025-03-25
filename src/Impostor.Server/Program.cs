using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Impostor.Api.Config;
using Impostor.Api.Events.Managers;
using Impostor.Api.Extension;
using Impostor.Api.Extension.Commands;
using Impostor.Api.Extension.Utils;
using Impostor.Api.Games;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net.Manager;
using Impostor.Api.Utils;
using Impostor.Server.Commands;
using Impostor.Server.Controllers;
using Impostor.Server.Events;
using Impostor.Server.Events.Player;
using Impostor.Server.Hubs;
using Impostor.Server.Net;
using Impostor.Server.Net.Factories;
using Impostor.Server.Net.Manager;
using Impostor.Server.Net.Messages;
using Impostor.Server.Plugins;
using Impostor.Server.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Next.Hazel.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;
using Scalar.AspNetCore;

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

    private static string? GetArg(this string[] args, string name)
    {
        if (!args.Contains(name))
        {
            return null;
        }

        var index = Array.IndexOf(args, name);
        return index + 1 < args.Length ? args[index + 1] : null;
    }

    private static IConfiguration CreateConfiguration(string[] args)
    {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder.SetBasePath(args.GetArg("--base") ?? Directory.GetCurrentDirectory());
        configurationBuilder.AddJsonFile(args.GetArg("--config") ?? "config.json", true);
        configurationBuilder.AddEnvironmentVariables(args.GetArg("--prefix") ?? "IMPOSTOR_");
        configurationBuilder.AddCommandLine(args);

        return configurationBuilder.Build();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var configuration = CreateConfiguration(args)
            .GetConfig<ServerConfig>(ServerConfig.Section, out var serverConfig)
            .GetConfig<ExtensionServerConfig>(ExtensionServerConfig.Section, out var extensionConfig)
            .GetConfig<PluginConfig>(PluginConfig.Section, out var pluginConfig);

        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureServer(configuration, serverConfig)
            .ConfigureExtension(extensionConfig)
            .ConfigureLog(serverConfig)
            .UseContentRoot(serverConfig.RootPath ?? Directory.GetCurrentDirectory())
            .UseEnvironment(serverConfig.Env ?? DotnetUtils.Environment)
            .UsePluginLoader(pluginConfig)
            .UseConsoleLifetime();

        return hostBuilder;
    }

    private static IHostBuilder ConfigureServer(this IHostBuilder builder, IConfiguration configuration,
        ServerConfig config)
    {
        builder.ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddConfiguration(configuration);
            })
            .ConfigureServices((host, services) =>
            {
                services
                    .AddHazel()
                    .AddPolicy<PlayerMovementEvent.PlayerMovementEventObjectPolicy, PlayerMovementEvent>();

                services
                    .ConfigureSection<AntiCheatConfig>(host.Configuration, AntiCheatConfig.Section)
                    .ConfigureSection<CompatibilityConfig>(host.Configuration, CompatibilityConfig.Section)
                    .ConfigureSection<ServerConfig>(host.Configuration, ServerConfig.Section)
                    .ConfigureSection<TimeoutConfig>(host.Configuration, TimeoutConfig.Section)
                    .ConfigureSection<PluginConfig>(host.Configuration, PluginConfig.Section)
                    .ConfigureSection<ExtensionServerConfig>(host.Configuration, ExtensionServerConfig.Section);

                services
                    .AddSingleton(WebHub.WebSink.Sink)
                    .AddSingleton<HttpConnectionManager>()
                    .AddSingleton<ClientAuthManager>()
                    .AddSingleton<IMessageWriterProvider, MessageWriterProvider>()
                    .AddSingleton<IGameCodeFactory, GameCodeFactory>()
                    .AddSingleton<IEventManager, EventManager>()
                    .AddSingleton<IDateTimeProvider, RealDateTimeProvider>()
                    .AddSingleton<ICompatibilityManager, CompatibilityManager>()
                    .AddSingleton<IClientFactory, ClientFactory<Client>>();

                services
                    .AddRequiredSingleton<IClientManager, ClientManager>()
                    .AddRequiredSingleton<IGameManager, GameManager>()
                    .AddRequiredSingleton<ICommandManager, CommandManager>()
                    .AddRequiredSingleton<INetListenerManager, NetListenerManager>();

                if (config.EnableCommands)
                {
                    services.AddHostedService<ConsoleCommandService>();
                }

                if (config.EnableNextApi)
                {
                    services.AddHostedService<NetApiService>();
                }

                services.AddHostedService<StarterService>();
            });
        return builder;
    }

    private static IHostBuilder ConfigureLog(this IHostBuilder hostBuilder, ServerConfig serverConfig)
    {
        hostBuilder.UseSerilog((context, loggerConfiguration) =>
        {
            AssemblyLoadContext.Default.Resolving += LoadSerilogAssembly;

            loggerConfiguration
                .MinimumLevel.Is(serverConfig.LogLevel)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .LoggerSet(serverConfig)
                .WriteTo.Sink(WebHub.WebSink.Sink, serverConfig.LogLevel)
                .ReadFrom.Configuration(context.Configuration,
                    new ConfigurationReaderOptions(ConfigurationAssemblySource.AlwaysScanDllFiles));

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

    private static LoggerConfiguration LoggerSet(this LoggerConfiguration config, ServerConfig serverConfig)
    {
        return serverConfig switch
        {
            { WriteConsole: true, WriteFile: true } => config.WriteTo.Console().WriteTo.File(serverConfig.LogFilePath),
            { WriteConsole: false, WriteFile: true } => config.WriteTo.File(serverConfig.LogFilePath),
            { WriteConsole: true, WriteFile: false } => config.WriteTo.Console(),
            _ => config,
        };
    }

    internal static UUID CurrentUuid = UUID.New();
    private static IHostBuilder ConfigureExtension(this IHostBuilder builder, ExtensionServerConfig config)
    {
        if (!config.Enabled)
        {
            return builder;
        }

        Log.Information("Enable Server Extension");
        return builder.ConfigureWebHostDefaults(hostBuilder =>
        {
            if (config.EnabledSpa)
            {
                hostBuilder.UseWebRoot(config.SpaDirectory);
            }
            
            hostBuilder.ConfigureKestrel(options =>
            {
                Log.Information("Http Listen {ip} {port}", config.ListenIp, config.ListenPort);
                options.Listen(IPAddress.Parse(config.ListenIp.ResolveIp()), config.ListenPort);
            });
            
            hostBuilder.ConfigureServices((host,services) =>
            {
                if (config.EnabledSignalR)
                {
                    services.AddSignalR();
                }
                
                if (config.EnabledHttpApi)
                {
                    services
                        .AddControllers()
                        .ConfigurePluginMvc();

                    if (config.UseAuth)
                    {
                        services
                            .AddAuthentication()
                            .AddJwtBearer(options =>
                            { 
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = false,
                                    ValidateAudience = false,
                                    ValidateIssuerSigningKey = true,
                                    ValidateLifetime = true,
                                    IssuerSigningKey =
                                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CurrentUuid.ToString())),
                                };
                            })
                            .Services
                            .AddSingleton<JwtSecurityTokenHandler>();
                    }

                    if (host.HostingEnvironment.IsDevelopment())
                    {
                        services.AddOpenApi();
                    }
                }

                if (config.EnabledWebSocket)
                {
                    services.AddWebSockets(option =>
                    {
                        option.KeepAliveTimeout = TimeSpan.FromSeconds(config.WebSocketTimeout);
                        option.KeepAliveInterval = TimeSpan.FromSeconds(config.WebSocketInterval);
                    });
                }

                if (config.EnabledSpa)
                {
                    services.AddSpaStaticFiles(configuration =>
                    {
                        configuration.RootPath = config.SpaDirectory;
                    });
                }
            });


            hostBuilder.Configure((builderContext,applicationBuilder) =>
            {
                var isDev = builderContext.HostingEnvironment.IsDevelopment();
                
                applicationBuilder.ConfigurePluginWeb(hostBuilder);

                if (config is { EnabledHttpApi: true, UseAuth: true })
                {
                    applicationBuilder
                        .UseAuthentication()
                        .UseAuthorization();
                }

                if (config.EnabledWebSocket)
                {
                    Log.Information("Enable Websocket");
                    applicationBuilder.UseWebSockets();
                }
                
                if (config.EnabledSpa)
                {
                    Log.Information("Enable Spa");
                    applicationBuilder.Map("/web", webBuilder =>
                    {
                        if (isDev)
                        {
                            webBuilder.UseSpa(spa =>
                            {
                                spa.UseProxyToSpaDevelopmentServer("http://localhost:5173");
                            });
                        }

                        if (isDev || !Directory.Exists(config.SpaDirectory))
                        {
                            return;
                        }
                        
                        webBuilder.UseSpaStaticFiles();
                        webBuilder.UseSpa(configuration =>
                        {
                        });
                    });
                }

                applicationBuilder.UseRouting();
                applicationBuilder.UseEndpoints(endpoint =>
                {
                    if (config.EnabledSignalR)
                    {
                        Log.Information("Enable SignalR");
                        endpoint.MapHub<WebHub>("/signalr/web");
                    }

                    if (config.EnabledHttpApi)
                    {
                        Log.Information("Enable Http Api");
                        endpoint.MapControllers();

                        if (isDev)
                        {
                            Log.Information("Map OpenApi and Scalar");
                            endpoint.MapScalarApiReference();
                            endpoint.MapOpenApi();
                        }
                    }
                });
            });
        });
    }

    private static IConfiguration GetConfig<T>(this IConfiguration configuration, string section, out T result)
        where T : class, new()
    {
        result = configuration.GetSection(section)
            .Get<T>() ?? new T();
        return configuration;
    }
}
