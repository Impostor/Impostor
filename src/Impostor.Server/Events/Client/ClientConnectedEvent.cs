using Impostor.Api.Events.Client;
using Impostor.Api.Net;

namespace Impostor.Server.Events.Client;

public class ClientConnectedEvent(IHazelConnection connection, IClient client) : IClientConnectedEvent
{
    public IHazelConnection Connection { get; } = connection;

    public IClient Client { get; } = client;
}
