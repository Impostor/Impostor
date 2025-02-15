using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Config;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net;

public class StarterService(NetListenerManager listenerManager, IOptions<ServerConfig> serverConfigOption)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serverConfig = serverConfigOption.Value;

        var index = 0;
        foreach (var listener in serverConfig.Listeners)
        {
            index++;
            listenerManager.Create(listener, index);
        }

        await listenerManager.StartAllAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await listenerManager.StopAllAsync();
    }
}
