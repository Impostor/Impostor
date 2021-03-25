using System.Collections.Generic;

namespace Impostor.Api.Net.Manager
{
    public interface IClientManager
    {
        IEnumerable<IClient> Clients { get; }
    }
}
