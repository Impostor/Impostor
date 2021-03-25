using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Tools.ServerReplay.Mocks
{
    public class MockHazelConnection : IHazelConnection
    {
        public MockHazelConnection(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            IsConnected = true;
            Client = null;
        }

        public IPEndPoint EndPoint { get; }
        public bool IsConnected { get; }
        public IClient Client { get; set; }

        public ValueTask SendAsync(IMessageWriter writer)
        {
            return ValueTask.CompletedTask;
        }

        public ValueTask DisconnectAsync(string reason)
        {
            return ValueTask.CompletedTask;
        }
    }
}
