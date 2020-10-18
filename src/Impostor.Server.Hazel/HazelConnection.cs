using System;
using System.Net;
using System.Threading.Tasks;
using Hazel;
using Impostor.Server.Hazel.Messages;
using Impostor.Server.Net;
using Impostor.Server.Net.Messages;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Hazel
{
    internal class HazelConnection : IConnection
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

        public IClient Client { get; set; }

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
                _logger.LogWarning("Client was null.");
                return;
            }

            while (true)
            {
                if (e.Message.Position >= e.Message.Length)
                {
                    break;
                }

                var reader = e.Message.ReadMessage();
                var type = e.SendOption switch
                {
                    SendOption.None => MessageType.Unreliable,
                    SendOption.Reliable => MessageType.Reliable,
                    _ => throw new NotSupportedException()
                };

                using var message = new HazelMessage(reader, type);

                await Client.HandleMessageAsync(message);
            }
        }

        public IConnectionMessageWriter CreateMessage(MessageType messageType)
        {
            return new HazelConnectionMessageWriter(messageType, this);
        }
    }
}