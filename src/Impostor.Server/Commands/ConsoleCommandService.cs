using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Commands;

public class ConsoleCommandService(ILogger<ConsoleCommandService> logger, ICommandsManager commandsManager) : BackgroundService
{
    private TextReader Reader { get; } = Console.In;
    
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Console.OutputEncoding = Console.InputEncoding = Encoding.UTF8;
        
        logger.LogInformation("Starting ConsoleCommandService");
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var line = await Reader.ReadLineAsync(stoppingToken);
            if (line == null) continue;
            logger.LogDebug("Received input: {line}", line);
            
            if (!line.StartsWith('/')) continue;
            await HandleCommandAsync(line[1..]);
        }
    }

    private async Task HandleCommandAsync(string command)
    {
        var args = command.Split(" ");
    }
}
