using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.Net.Factories
{
    internal class ClientFactory<TClient> : IClientFactory
        where TClient : ClientBase
    {
        private readonly IServiceProvider _serviceProvider;

        public ClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IClient Create(IConnection connection, string name, int clientVersion)
        {
            var client = ActivatorUtilities.CreateInstance<TClient>(_serviceProvider, name, connection);
            connection.Client = client;
            return client;
        }
    }
}