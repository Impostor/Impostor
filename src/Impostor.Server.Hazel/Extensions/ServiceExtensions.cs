using Impostor.Server.Net.Manager;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.Hazel
{
    public static class ServiceExtensions
    {
        public static IServiceCollection UseHazelMatchmaking(this IServiceCollection services)
        {
            services.AddSingleton<IMatchmaker, HazelMatchmaker>();
            return services;
        }
    }
}