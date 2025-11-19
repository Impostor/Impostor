using Impostor.Api.Events;
using Impostor.Api.Plugins;
using Impostor.Plugins.MorePlayers.Configuration;
using Impostor.Plugins.MorePlayers.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Plugins.MorePlayers
{
    public class MorePlayersStartup : IPluginStartup
    {
        public void ConfigureHost(IHostBuilder host)
        {
            // Configure additional host settings if needed
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register configuration
            services.Configure<MorePlayersConfig>(config =>
            {
                // Default configuration values (can be overridden by appsettings.json)
                config.MaxPlayers = 20;
                config.ImpostorRatio = 5;
                config.AutoCreateGame = true;
                config.GameName = "High Capacity Game";
                config.MakeGamePublic = false;
            });

            // Register event listeners
            services.AddSingleton<IEventListener, GameEventHandler>();
            services.AddSingleton<IEventListener, PlayerEventHandler>();
        }
    }
}
