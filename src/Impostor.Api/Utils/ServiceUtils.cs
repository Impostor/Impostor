using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Api.Utils;

public static class ServiceUtils
{
    public static IServiceCollection AddRequiredSingleton<TInterface, TImplementation>(this IServiceCollection services)
        where TImplementation : class, TInterface where TInterface : class
    {
        return services
            .AddSingleton<TImplementation>()
            .AddSingleton<TInterface>(p => p.GetRequiredService<TImplementation>());
    }
}
