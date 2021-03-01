using Impostor.Server.Events.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Server
{
    public static class ServiceProviderExtensions
    {
        public static void AddEventPools(this IServiceCollection services)
        {
            services.TryAddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());

            services.AddSingleton(serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                var policy = ActivatorUtilities.CreateInstance<PlayerMovementEvent.PlayerMovementEventObjectPolicy>(serviceProvider);
                return provider.Create(policy);
            });
        }
    }
}
