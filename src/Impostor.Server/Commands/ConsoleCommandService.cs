using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Api.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Commands;

public class ConsoleCommandService(
    IServiceProvider serviceProvider,
    ILogger<ConsoleCommandService> logger,
    IOptions<ServerConfig> config,
    CommandManager commandManager) : BackgroundService
{
    private readonly ServerConfig _config = config.Value;
    private TextReader Reader { get; } = Console.In;

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Console.OutputEncoding = Console.InputEncoding = Encoding.UTF8;

        logger.LogInformation("Starting ConsoleCommandService");
        foreach (var command in serviceProvider.GetServices<ICommand>())
        {
            commandManager.RegisterCommand(command);
        }

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var line = await Reader.ReadLineAsync(stoppingToken);
            if (line == null)
            {
                continue;
            }

            logger.LogDebug("Received input: {line}", line);

            var prefix = _config.CommandPrefix;
            var trimLine = line.Trim();
            if (!trimLine.StartsWith(prefix))
            {
                continue;
            }

            var command = trimLine.Remove(0, prefix.Length);
            await commandManager.HandleCommandAsync(command);
        }
    }

    internal async ValueTask<bool> HandleDefaultCommandAsync(string command, string[] args)
    {
        return false;
    }
}
