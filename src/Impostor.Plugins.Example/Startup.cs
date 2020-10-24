using System;
using Impostor.Api.Events;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Plugins.Example
{
    public class Startup : IPluginStartup
    {
        public void ConfigureHost(IHostBuilder host)
        {
            Console.WriteLine("[Example] Host configured");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IEventListener, CatchAllEventListener>();
            Console.WriteLine("[Example] Services configured");
        }
    }
}
