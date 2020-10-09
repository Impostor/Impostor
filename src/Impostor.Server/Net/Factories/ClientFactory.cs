using System;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.Net.Factories
{
    internal class ClientFactory<TClient> : IClientFactory
        where TClient : ClientBase
    {
        private int _idLast;
        private readonly IServiceProvider _serviceProvider;

        public ClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public int NextId()
        {
            var clientId = Interlocked.Increment(ref _idLast);

            if (clientId < 1)
            {
                // Super rare but reset the _idLast because of overflow.
                _idLast = 0;
                
                // And get a new id.
                clientId = Interlocked.Increment(ref _idLast);
            }

            return clientId;
        }

        public async ValueTask<IClient> CreateAsync(IConnection connection, string name, int clientVersion)
        {
            if (clientVersion != 50516550)
            {
                using var packet = connection.CreateMessage(MessageType.Reliable);
                Message01JoinGame.SerializeError(packet, false, DisconnectReason.IncorrectVersion);
                await packet.SendAsync();

                throw new ClientVersionUnsupportedException(clientVersion);
            }
            
            var clientId = NextId();
            var client = ActivatorUtilities.CreateInstance<TClient>(_serviceProvider, clientId, name, connection);

            await client.InitializeAsync();
            
            return client;
        }
    }
}