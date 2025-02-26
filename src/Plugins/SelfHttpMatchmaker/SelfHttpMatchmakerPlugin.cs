using Impostor.Api.Extension;
using Impostor.Api.Plugins;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace SelfHttpMatchmaker;

[ImpostorPlugin("SelfHttpMatchmaker.Impostor.Next")]
public class SelfHttpMatchmakerPlugin : IHttpPlugin, IHttpPluginStartup
{
    public void ConfigureHost(IWebHostBuilder host)
    {
        host.ConfigureServices(service =>
        {
            service.AddMvc().AddApplicationPart(typeof(SelfHttpMatchmakerPlugin).Assembly);
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ListingManager>();
    }
}
