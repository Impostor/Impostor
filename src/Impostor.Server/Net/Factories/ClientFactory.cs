using System;
using System.Collections.Generic;
using Impostor.Api.Net;
using Impostor.Api.Reactor;
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

        public ClientBase Create(IHazelConnection connection, string name, int clientVersion, ISet<Mod> mods)
        {
            var client = ActivatorUtilities.CreateInstance<TClient>(_serviceProvider, name, clientVersion, connection, mods);
            connection.Client = client;
            return client;
        }
    }
}
