using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Hazel
{
    internal class HazelConnection : IHazelConnection
    {
        private readonly ILogger<HazelConnection> _logger;

        public HazelConnection(Connection innerConnection, ILogger<HazelConnection> logger)
        {
            _logger = logger;
            InnerConnection = innerConnection;
            innerConnection.DataReceived = ConnectionOnDataReceived;
            innerConnection.Disconnected = ConnectionOnDisconnected;
        }

        public Connection InnerConnection { get; }

        public IPEndPoint EndPoint => InnerConnection.EndPoint;

        public bool IsConnected => InnerConnection.State == ConnectionState.Connected;

        public IClient? Client { get; set; }

        public ValueTask SendAsync(IMessageWriter writer)
        {
            return InnerConnection.SendAsync(writer);
        }

        public ValueTask DisconnectAsync(string? reason)
        {
            return InnerConnection.Disconnect(reason);
        }

        public void DisposeInnerConnection()
        {
            InnerConnection.Dispose();
        }

        private async ValueTask ConnectionOnDisconnected(DisconnectedEventArgs e)
        {
            if (Client != null)
            {
                await Client.HandleDisconnectAsync(e.Reason);
            }
        }

        private async ValueTask ConnectionOnDataReceived(DataReceivedEventArgs e)
        {
            if (Client == null)
            {
                return;
            }

            while (true)
            {
                if (e.Message.Position >= e.Message.Length)
                {
                    break;
                }

                using (var message = e.Message.ReadMessage())
                {
                    await Client.HandleMessageAsync(message, e.Type);
                }
            }
        }
    }
}
