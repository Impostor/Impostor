using Impostor.Api.Events.Client;
using Impostor.Api.Net;

namespace Impostor.Server.Events.Client
{
    public class ClientConnectionEvent : IClientConnectionEvent
    {
        public ClientConnectionEvent(IHazelConnection connection, IMessageReader handshakeData)
        {
            Connection = connection;
            HandshakeData = handshakeData;
        }

        public IHazelConnection Connection { get; }

        public IMessageReader HandshakeData { get; }
    }
}
