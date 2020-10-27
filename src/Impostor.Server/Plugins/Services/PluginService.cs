using System.Threading;
using System.Threading.Tasks;
using Impostor.Server.Plugins.Managers;
using Microsoft.Extensions.Hosting;

namespace Impostor.Server.Plugins.Services
{
    public class PluginService : IHostedService
    {
        private readonly PluginManager _pluginManager;

        public PluginService(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _pluginManager.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _pluginManager.StopAsync();
        }
    }
}
