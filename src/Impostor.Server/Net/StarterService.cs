namespace Impostor.Server.Net;

using System.Threading;
using System.Threading.Tasks;
using Api.Config;
using Manager;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

public class StarterService(NetListenerManager listenerManager, IOptions<ServerConfig> serverConfigOption) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serverConfig = serverConfigOption.Value;

        await listenerManager.StartAllAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await listenerManager.StopAllAsync();
    }
}
