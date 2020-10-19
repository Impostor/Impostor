using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Impostor.Server.Net.Manager
{
    internal partial class ClientManager : IClientManager
    {
        IEnumerable<IClient> IClientManager.Clients => _clients.Values;
    }
}