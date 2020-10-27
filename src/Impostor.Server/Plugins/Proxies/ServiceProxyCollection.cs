using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.Plugins.Proxies
{
    public record ServiceRegistration(Type Type, ServiceLifetime Lifetime);

    public class ServiceProxyCollection
    {
        public ServiceProxyCollection(IReadOnlyList<ServiceRegistration> serviceTypes)
        {
            ServiceTypes = serviceTypes;
        }

        public IReadOnlyList<ServiceRegistration> ServiceTypes { get; }
    }
}
