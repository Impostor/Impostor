using Impostor.Api.Extension;
using Impostor.Api.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SelfHttpMatchmaker;

[ImpostorPlugin("SelfHttpMatchmaker.Impostor.Next")]
public class SelfHttpMatchmakerPlugin : IPlugin, IHttpPluginStartup
{
    public bool AssemblyPart => true;
    

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ListingManager>();
    }
}
