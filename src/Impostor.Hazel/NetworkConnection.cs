using System;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;

namespace Impostor.Hazel
{
    public enum HazelInternalErrors
    {
        SocketExceptionSend,
        SocketExceptionReceive,
        ReceivedZeroBytes,
        PingsWithoutResponse,
        ReliablePacketWithoutResponse,
        ConnectionDisconnected
    }

    /// <summary>
    ///     Abstract base class for a <see cref="Connection"/> to a remote end point via a network protocol like TCP or UDP.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public abstract class NetworkConnection : Connection
    {
        /// <summary>
        /// An event that gives us a chance to send well-formed disconnect messages to clients when an internal disconnect happens.
        /// </summary>
        public Func<HazelInternalErrors, MessageWriter> OnInternalDisconnect;

        /// <summary>
        ///     The remote end point of this connection.
        /// </summary>
        /// <remarks>
        ///     This is the end point of the other device given as an <see cref="System.Net.EndPoint"/> rather than a generic
        ///     <see cref="ConnectionEndPoint"/> as the base <see cref="Connection"/> does.
        /// </remarks>
        public IPEndPoint RemoteEndPoint { get; protected set; }

        public long GetIP4Address()
        {
            var bytes = this.RemoteEndPoint.Address.GetAddressBytes();

            return IPMode == IPMode.IPv4
                ? (uint)((bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0]) & 0x0FFFFFFFF)
                : BitConverter.ToInt64(bytes, bytes.Length - 8);
        }

        /// <summary>
        ///     Sends a disconnect message to the end point.
        /// </summary>
        protected abstract ValueTask<bool> SendDisconnect(MessageWriter writer);

        /// <summary>
        ///     Called when the socket has been disconnected at the remote host.
        /// </summary>
        protected async ValueTask DisconnectRemote(string reason, IMessageReader reader)
        {
            if (await SendDisconnect(null))
            {
                try
                {
                    await InvokeDisconnected(reason, reader);
                }
                catch { }
            }

            this.Dispose();
        }

        /// <summary>
        /// Called when socket is disconnected internally
        /// </summary>
        internal async ValueTask DisconnectInternal(HazelInternalErrors error, string reason)
        {
            var handler = this.OnInternalDisconnect;
            if (handler != null)
            {
                MessageWriter messageToRemote = handler(error);
                if (messageToRemote != null)
                {
                    try
                    {
                        await Disconnect(reason, messageToRemote);
                    }
                    finally
                    {
                        messageToRemote.Recycle();
                    }
                }
                else
                {
                    await Disconnect(reason);
                }
            }
            else
            {
                await Disconnect(reason);
            }
        }

        /// <summary>
        ///     Called when the socket has been disconnected locally.
        /// </summary>
        public override async ValueTask Disconnect(string reason, MessageWriter writer = null)
        {
            if (await SendDisconnect(writer))
            {
                try
                {
                    await InvokeDisconnected(reason, null);
                }
                catch { }
            }

            this.Dispose();
        }
    }
}
