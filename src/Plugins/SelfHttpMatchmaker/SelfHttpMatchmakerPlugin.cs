using Impostor.Api.Extension;
using Impostor.Api.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace SelfHttpMatchmaker;

[ImpostorPlugin("SelfHttpMatchmaker.Impostor.Next")]
public class SelfHttpMatchmakerPlugin : IHttpPlugin, IHttpPluginStartup
{
    public bool AssemblyPart
    {
        get => true;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ListingManager>();
    }
}
