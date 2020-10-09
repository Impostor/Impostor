using System;
using System.Net;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Hazel;
using Impostor.Server.Net;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Hazel
{
    internal class HazelConnection : IConnection
    {
        private readonly ILogger<HazelConnection> _logger;
        private readonly ConcurrentSimpleAsyncSubject<IMessage> _messageReceived = new ConcurrentSimpleAsyncSubject<IMessage>();

        public HazelConnection(Connection innerConnection, ILogger<HazelConnection> logger)
        {
            _logger = logger;
            InnerConnection = innerConnection;
            innerConnection.DataReceived += ConnectionOnDataReceived;
            innerConnection.Disconnected += ConnectionOnDisconnected;
        }
        
        public Connection InnerConnection { get; }

        public IAsyncObservable<IMessage> MessageReceived => _messageReceived;

        public IPEndPoint EndPoint => InnerConnection.EndPoint;

        public bool IsConnected => InnerConnection.State == ConnectionState.Connected;

        private void ConnectionOnDisconnected(object sender, DisconnectedEventArgs e)
        {
            Task.Run(_messageReceived.OnCompletedAsync);
        }

        private void ConnectionOnDataReceived(DataReceivedEventArgs e)
        {
            Task.Run(() => HandleData(e));
        }

        private async Task HandleData(DataReceivedEventArgs e)
        {
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

                    await _messageReceived.OnNextAsync(message);
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

        public IConnectionMessageWriter CreateMessage(MessageType type)
        {
            return new HazelConnectionMessageWriter(type, InnerConnection);
        }
    }
}