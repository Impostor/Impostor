using System;
using Impostor.Server.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Impostor.Plugins.Debugger
{
    public class DebugPlugin : PluginBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
        }

        public override void ConfigureHost(IHostBuilder host)
        {
            host.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.Configure(app =>
                {
                    app.UseStaticFiles();
                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapBlazorHub();
                        endpoints.MapFallbackToPage("/_Host");
                    });
                });
            });
        }
    }
}