using System.Collections.Generic;
using Impostor.Api.Net;
using Impostor.Api.Net.Manager;

namespace Impostor.Server.Net.Manager
{
    internal partial class ClientManager : IClientManager
    {
        IEnumerable<IClient> IClientManager.Clients => _clients.Values;
    }
}
