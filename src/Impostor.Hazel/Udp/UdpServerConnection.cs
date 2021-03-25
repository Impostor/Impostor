using System;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Hazel.Udp
{
    /// <summary>
    ///     Represents a servers's connection to a client that uses the UDP protocol.
    /// </summary>
    /// <inheritdoc/>
    internal sealed class UdpServerConnection : UdpConnection
    {
        /// <summary>
        ///     The connection listener that we use the socket of.
        /// </summary>
        /// <remarks>
        ///     Udp server connections utilize the same socket in the listener for sends/receives, this is the listener that 
        ///     created this connection and is hence the listener this conenction sends and receives via.
        /// </remarks>
        public UdpConnectionListener Listener { get; private set; }

        /// <summary>
        ///     Creates a UdpConnection for the virtual connection to the endpoint.
        /// </summary>
        /// <param name="listener">The listener that created this connection.</param>
        /// <param name="endPoint">The endpoint that we are connected to.</param>
        /// <param name="IPMode">The IPMode we are connected using.</param>
        internal UdpServerConnection(UdpConnectionListener listener, IPEndPoint endPoint, IPMode IPMode, ObjectPool<MessageReader> readerPool) : base(listener, readerPool)
        {
            this.Listener = listener;
            this.RemoteEndPoint = endPoint;
            this.EndPoint = endPoint;
            this.IPMode = IPMode;

            State = ConnectionState.Connected;
            this.InitializeKeepAliveTimer();
        }

        /// <inheritdoc />
        protected override async ValueTask WriteBytesToConnection(byte[] bytes, int length)
        {
            await Listener.SendData(bytes, length, RemoteEndPoint);
        }

        /// <inheritdoc />
        /// <remarks>
        ///     This will always throw a HazelException.
        /// </remarks>
        public override ValueTask ConnectAsync(byte[] bytes = null)
        {
            throw new InvalidOperationException("Cannot manually connect a UdpServerConnection, did you mean to use UdpClientConnection?");
        }

        /// <summary>
        ///     Sends a disconnect message to the end point.
        /// </summary>
        protected override async ValueTask<bool> SendDisconnect(MessageWriter data = null)
        {
            lock (this)
            {
                if (this._state != ConnectionState.Connected) return false;
                this._state = ConnectionState.NotConnected;
            }
            
            var bytes = EmptyDisconnectBytes;
            if (data != null && data.Length > 0)
            {
                if (data.SendOption != MessageType.Unreliable) throw new ArgumentException("Disconnect messages can only be unreliable.");

                bytes = data.ToByteArray(true);
                bytes[0] = (byte)UdpSendOption.Disconnect;
            }

            try
            {
                await Listener.SendData(bytes, bytes.Length, RemoteEndPoint);
            }
            catch { }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            Listener.RemoveConnectionTo(RemoteEndPoint);

            if (disposing)
            {
                _ = SendDisconnect();
            }

            base.Dispose(disposing);
        }
    }
}
