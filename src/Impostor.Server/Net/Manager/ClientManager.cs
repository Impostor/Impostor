using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Server.Net.Factories;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth.Data;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Manager
{
    internal class ClientManager : IClientManager
    {
        private readonly ILogger<ClientManager> _logger;
        private readonly ConcurrentDictionary<int, IClient> _clients;
        private readonly IClientFactory _clientFactory;
        private int _idLast;

        public ClientManager(ILogger<ClientManager> logger, IClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _clients = new ConcurrentDictionary<int, IClient>();
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

        public async ValueTask RegisterConnectionAsync(IConnection connection, string name, int clientVersion)
        {
            // 50516550 = 2020.09.22
            // 50518400 = 2020.10.08
            if (clientVersion != 50516550 && clientVersion != 50518400)
            {
                using var packet = connection.CreateMessage(MessageType.Reliable);
                Message01JoinGame.SerializeError(packet, false, DisconnectReason.IncorrectVersion);
                await packet.SendAsync();
                return;
            }

            var client = _clientFactory.Create(connection, name, clientVersion);

            Register(client);

            await connection.ListenAsync();
        }

        public void Register(IClient client)
        {
            var id = NextId();

            client.Id = id;
            _logger.LogInformation("Client connected.");
            _clients.TryAdd(id, client);
        }

        public void Remove(IClient client)
        {
            _logger.LogInformation("Client disconnected.");
            _clients.TryRemove(client.Id, out _);
        }

        public bool Validate(IClient client)
        {
            return client.Id != 0
                   && _clients.TryGetValue(client.Id, out var registeredClient)
                   && ReferenceEquals(client, registeredClient);
        }
    }
}