namespace Impostor.Server.Net;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

public class NetApiService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
