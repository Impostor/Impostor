using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Api.Plugins
{
    public class PluginBase : IPlugin
    {
        public virtual ValueTask EnableAsync()
        {
            return default;
        }

        public virtual ValueTask DisableAsync()
        {
            return default;
        }

        public virtual ValueTask ReloadAsync()
        {
            return default;
        }

        public virtual void ConfigureHost(IHostBuilder host)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }
    }
}