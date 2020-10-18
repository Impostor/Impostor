using System.Collections.Generic;

namespace Impostor.Server.Net.Manager
{
    public interface IClientManager
    {
        IEnumerable<IClient> Clients { get; }
    }
}