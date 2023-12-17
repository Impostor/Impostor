using Microsoft.AspNetCore.Builder;

namespace Impostor.Api.Plugins;

public interface IPluginHttpStartup : IPluginStartup
{
    void ConfigureWebApplication(IApplicationBuilder builder);
}
