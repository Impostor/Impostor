using System.Collections.Concurrent;
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

        // No idea what a good way for this is.
        private int NextId()
        {
            lock (_idLock)
            {
                // 3 Attempts.
                for (var i = 0; i < 3; i++)
                {
                    // It is important that ids start from 1, a 0 id causes issues.
                    var result = ++_idLast;

                    if (_idLast == int.MaxValue)
                    {
                        _idLast = 0;
                    }

                    if (_clients.ContainsKey(result))
                    {
                        continue;
                    }
            
                    return result;
                }
                
                throw new AmongUsException("Unable to generate a client id.");
            }
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