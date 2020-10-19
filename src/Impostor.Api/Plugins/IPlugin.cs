using System.Threading.Tasks;
using Impostor.Server.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Server.Plugins
{
    public interface IPlugin : IEventListener
    {
        ValueTask EnableAsync();

        ValueTask DisableAsync();

        ValueTask ReloadAsync();

        void ConfigureHost(IHostBuilder host);

        void ConfigureServices(IServiceCollection services);
    }
}