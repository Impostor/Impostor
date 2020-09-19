using System.Collections.Generic;
using Serilog;

namespace AmongUs.Server.Net
{
    public class ClientManager
    {
        private static readonly ILogger Logger = Log.ForContext<ClientManager>();
        
        private readonly HashSet<Client> _clients;
        
        public ClientManager()
        {
            _clients = new HashSet<Client>();
        }
        
        public void Add(Client client)
        {
            Logger.Information("Client connected.");
            
            _clients.Add(client);
        }

        public void Remove(Client client)
        {
            Logger.Information("Client disconnected.");
            
            _clients.Remove(client);
        }
    }
}