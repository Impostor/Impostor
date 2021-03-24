using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Api.Plugins
{
    public interface IPluginStartup
    {
        void ConfigureHost(IHostBuilder host);

        void ConfigureServices(IServiceCollection services);
    }
}
