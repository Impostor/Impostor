using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Impostor.Server.Net.Factories;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Manager
{
    internal class ClientManager : IClientManager
    {
        private readonly ILogger<ClientManager> _logger;
        private readonly ConcurrentDictionary<int, IClient> _clients;
        private readonly IClientFactory _clientFactory;
        
        public ClientManager(ILogger<ClientManager> logger, IClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _clients = new ConcurrentDictionary<int, IClient>();
        }

        public async ValueTask RegisterConnectionAsync(IConnection connection, string name, int clientVersion)
        {
            try
            {
                var client = await _clientFactory.CreateAsync(connection, name, clientVersion);
                
                Register(client);
            }
            catch (ClientVersionUnsupportedException ex)
            {
                _logger.LogTrace("Closed connection because client version {Version} is not supported.", ex.Version);
            }
        }

        public void Register(IClient client)
        {
            _logger.LogInformation("Client connected.");
            _clients.TryAdd(client.Id, client);
        }

        public void Remove(IClient client)
        {
            _logger.LogInformation("Client disconnected.");
            _clients.TryRemove(client.Id, out _);
        }
    }
}