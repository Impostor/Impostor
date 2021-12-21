using Impostor.Api.Events;
using Impostor.Api.Plugins;
using Impostor.Plugins.Example.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Plugins.Example
{
    public class ExamplePluginStartup : IPluginStartup
    {
        public void ConfigureHost(IHostBuilder host)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IEventListener, GameEventListener>();
            services.AddSingleton<IEventListener, ClientEventListener>();
            services.AddSingleton<IEventListener, PlayerEventListener>();
            services.AddSingleton<IEventListener, MeetingEventListener>();
        }
    }
}
