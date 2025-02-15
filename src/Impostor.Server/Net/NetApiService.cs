using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Impostor.Server.Net;

public class NetApiService : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
