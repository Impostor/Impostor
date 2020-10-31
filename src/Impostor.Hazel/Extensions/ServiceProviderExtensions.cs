using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Hazel.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddHazel(this IServiceCollection services)
        {
            services.TryAddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());

            services.AddSingleton(serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                var policy = ActivatorUtilities.CreateInstance<MessageReaderPolicy>(serviceProvider);
                return provider.Create(policy);
            });
        }
    }
}
