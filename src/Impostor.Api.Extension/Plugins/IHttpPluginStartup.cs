using Impostor.Api.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Impostor.Api.Extension.Plugins;

public interface IHttpPluginStartup : IPluginStartup
{
    public virtual bool AssemblyPart { get => false; } 
    
    void ConfigureHost(IWebHostBuilder host) { }
    void ConfigureWebApplication(IApplicationBuilder app) { }
}
