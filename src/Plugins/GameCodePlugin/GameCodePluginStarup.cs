using Impostor.Api.Events;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace GameCodePlugin;

public class GameCodePluginStartup : IPluginStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton<IEventListener, GameCodeEventListener>()
            .AddSingleton<GameCodeStateManager>();
    }
}
