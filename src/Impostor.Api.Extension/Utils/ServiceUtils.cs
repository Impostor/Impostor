using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Api.Extension.Utils;

public static class ServiceUtils
{
    public static IServiceCollection ConfigureSection<T>(this IServiceCollection collection, IConfiguration configuration, string section) where T : class
    {
        return collection.Configure<T>(configuration.GetSection(section));
    }
}
