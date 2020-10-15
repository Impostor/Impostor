using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentStack<DataReceivedEventArgs> _pendingMessages;

        public HazelConnection(Connection innerConnection, ILogger<HazelConnection> logger)
        {
            _logger = logger;
            _pendingMessages = new ConcurrentStack<DataReceivedEventArgs>();
            InnerConnection = innerConnection;
            innerConnection.DataReceived += ConnectionOnDataReceived;
            innerConnection.Disconnected += ConnectionOnDisconnected;
        }

        public Connection InnerConnection { get; }

        public IPEndPoint EndPoint => InnerConnection.EndPoint;

        public bool IsConnected => InnerConnection.State == ConnectionState.Connected;

        public IClient Client { get; set; }

        private void ConnectionOnDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (Client != null)
            {
                Task.Run(Client.HandleDisconnectAsync);
            }
        }

        private void ConnectionOnDataReceived(DataReceivedEventArgs e)
        {
            Task.Run(() => HandleData(e));
        }

        private async Task HandleData(DataReceivedEventArgs e)
        {
            if (Client == null)
            {
                _pendingMessages.Push(e);
                return;
            }

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception caught in client data handler.");
            }
            finally
            {
                e.Message.Recycle();
            }
        }

        public IConnectionMessageWriter CreateMessage(MessageType messageType)
        {
            return new HazelConnectionMessageWriter(messageType, this);
        }

        public async ValueTask ListenAsync()
        {
            while (_pendingMessages.TryPop(out var eventArgs))
            {
                await HandleData(eventArgs);
            }
        }
    }
}