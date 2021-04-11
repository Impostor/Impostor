using Impostor.Api.Net;

namespace Impostor.Server.Net.Factories
{
    internal interface IClientFactory
    {
        ClientBase Create(IHazelConnection connection, string name, int clientVersion);
    }
}
