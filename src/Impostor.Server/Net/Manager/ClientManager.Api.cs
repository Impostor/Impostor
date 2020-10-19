using System.Collections.Generic;

namespace Impostor.Server.Net.Manager
{
    internal partial class ClientManager : IClientManager
    {
        IEnumerable<IClient> IClientManager.Clients => _clients.Values;
    }
}