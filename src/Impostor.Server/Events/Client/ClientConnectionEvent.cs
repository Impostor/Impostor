using Impostor.Api.Events.Client;
using Impostor.Api.Net;

namespace Impostor.Server.Events.Client;

public class ClientConnectionEvent(IHazelConnection connection, IMessageReader handshakeData) : IClientConnectionEvent
{
    public IHazelConnection Connection { get; } = connection;

    public IMessageReader HandshakeData { get; } = handshakeData;
}
