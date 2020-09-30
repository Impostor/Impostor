using System.Collections.Concurrent;
using System.Threading;
using Hazel;
using Impostor.Server.Exceptions;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Manager
{
    internal class ClientManager : IClientManager
    {
        private readonly ILogger<ClientManager> _clientManager;
        private readonly GameManager _gameManager;
        private readonly ConcurrentDictionary<int, Client> _clients;
        private readonly object _idLock;
        private int _idLast;
        
        public ClientManager(ILogger<ClientManager> clientManager, GameManager gameManager)
        {
            _clientManager = clientManager;
            _gameManager = gameManager;
            _clients = new ConcurrentDictionary<int, Client>();
            _idLock = new object();
            _idLast = 0;
        }

        private int NextId()
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
        
        public void Create(string name, Connection connection)
        {
            var clientId = NextId();
            
            _clientManager.LogInformation("Client connected.");
            _clients.TryAdd(clientId, new Client(this, _gameManager, clientId, name, connection));
        }

        public void Remove(Client client)
        {
            _clientManager.LogInformation("Client disconnected.");
            _clients.TryRemove(client.Id, out _);
        }
    }
}