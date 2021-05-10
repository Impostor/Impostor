using Impostor.Api.Events.Client;
using Impostor.Api.Net;

namespace Impostor.Server.Events.Client
{
    public class ClientConnectedEvent : IClientConnectedEvent
    {
        public ClientConnectedEvent(IHazelConnection connection, IClient client)
        {
            Connection = connection;
            Client = client;
        }

        public IHazelConnection Connection { get; }

        public IClient Client { get; }
    }
}
