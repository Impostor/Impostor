using System.Collections.Generic;
using Hazel;
using Impostor.Server.Net.Manager;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Redirector
{
    internal class ClientManagerRedirector : IClientManager
    {
        private readonly ILogger<ClientManagerRedirector> _logger;
        private readonly INodeProvider _nodeProvider;
        private readonly HashSet<ClientRedirector> _clients;

        public ClientManagerRedirector(ILogger<ClientManagerRedirector> logger, INodeProvider nodeProvider)
        {
            _logger = logger;
            _nodeProvider = nodeProvider;
            _clients = new HashSet<ClientRedirector>();
        }
        
        public void Create(string name, Connection connection)
        {
            _logger.LogInformation("Client connected.");
            _clients.Add(new ClientRedirector(name, connection, this, _nodeProvider));
        }

        public void Remove(ClientRedirector client)
        {
            _logger.LogInformation("Client disconnected.");
            _clients.Remove(client);
        }
    }
}