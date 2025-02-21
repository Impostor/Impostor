using Impostor.Api.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Impostor.Api.Extension;

public interface IHttpPluginStartup : IPluginStartup
{
    void ConfigureHost(IWebHostBuilder host) { }
    void ConfigureWebApplication(IApplicationBuilder app) { }
}
