using System.Collections.Concurrent;
using Impostor.Server.Exceptions;
using Serilog;

namespace Impostor.Server.Net.Manager
{
    public class ClientManager
    {
        private static readonly ILogger Logger = Log.ForContext<ClientManager>();
        
        private readonly ConcurrentDictionary<int, Client> _clients;
        private readonly object _idLock;
        private int _idLast;
        
        public ClientManager()
        {
            _clients = new ConcurrentDictionary<int, Client>();
            _idLock = new object();
            _idLast = 0;
        }

        // No idea what a good way for this is.
        public int NextId()
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

                    if (_clients.ContainsKey(_idLast))
                    {
                        continue;
                    }
            
                    return result;
                }
                
                throw new AmongUsException("Unable to generate a client id.");
            }
        }
        
        public void Add(Client client)
        {
            Logger.Information("Client connected.");

            _clients.TryAdd(client.Id, client);
        }

        public void Remove(Client client)
        {
            Logger.Information("Client disconnected.");

            _clients.TryRemove(client.Id, out _);
        }
    }
}