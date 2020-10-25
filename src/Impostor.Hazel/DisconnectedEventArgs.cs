using System;
using Impostor.Api.Net.Messages;

namespace Impostor.Hazel
{
    public class DisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Optional disconnect reason. May be null.
        /// </summary>
        public readonly string Reason;

        /// <summary>
        /// Optional data sent with a disconnect message. May be null. 
        /// You must not recycle this. If you need the message outside of a callback, you should copy it.
        /// </summary>
        public readonly IMessageReader Message;

        public DisconnectedEventArgs(string reason, IMessageReader message)
        {
            this.Reason = reason;
            this.Message = message;
        }
    }
}
