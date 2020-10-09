using System;
using System.Net;

namespace Impostor.Server.Net
{
    public interface IConnection
    {
        IAsyncObservable<IMessage> MessageReceived { get; }
        
        IPEndPoint EndPoint { get; }
        
        bool IsConnected { get; }

        IConnectionMessageWriter CreateMessage(MessageType type);
    }
}