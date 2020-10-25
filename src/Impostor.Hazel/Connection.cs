using System;
using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;
using Serilog;

namespace Impostor.Hazel
{
    /// <summary>
    ///     Base class for all connections.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Connection is the base class for all connections that Hazel can make. It provides common functionality and a 
    ///         standard interface to allow connections to be swapped easily.
    ///     </para>
    ///     <para>
    ///         Any class inheriting from Connection should provide the 3 standard guarantees that Hazel provides:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>Thread Safe</description>
    ///             </item>
    ///             <item>
    ///                 <description>Connection Orientated</description>
    ///             </item>
    ///             <item>
    ///                 <description>Packet/Message Based</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true"/>
    public abstract class Connection : IDisposable
    {
        private static readonly ILogger Logger = Log.ForContext<Connection>();

        /// <summary>
        ///     Called when a message has been received.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         DataReceived is invoked everytime a message is received from the end point of this connection, the message 
        ///         that was received can be found in the <see cref="DataReceivedEventArgs"/> alongside other information from the 
        ///         event.
        ///     </para>
        ///     <include file="DocInclude/common.xml" path="docs/item[@name='Event_Thread_Safety_Warning']/*" />
        /// </remarks>
        /// <example>
        ///     <code language="C#" source="DocInclude/TcpClientExample.cs"/>
        /// </example>
        public Func<DataReceivedEventArgs, ValueTask> DataReceived;

        public int TestLagMs = -1;
        public int TestDropRate = 0;
        protected int testDropCount = 0;
        
        /// <summary>
        ///     Called when the end point disconnects or an error occurs.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Disconnected is invoked when the connection is closed due to an exception occuring or because the remote 
        ///         end point disconnected. If it was invoked due to an exception occuring then the exception is available 
        ///         in the <see cref="DisconnectedEventArgs"/> passed with the event.
        ///     </para>
        ///     <include file="DocInclude/common.xml" path="docs/item[@name='Event_Thread_Safety_Warning']/*" />
        /// </remarks>
        /// <example>
        ///     <code language="C#" source="DocInclude/TcpClientExample.cs"/>
        /// </example>
        public Func<DisconnectedEventArgs, ValueTask> Disconnected;

        /// <summary>
        ///     The remote end point of this Connection.
        /// </summary>
        /// <remarks>
        ///     This is the end point that this connection is connected to (i.e. the other device). This returns an abstract 
        ///     <see cref="ConnectionEndPoint"/> which can then be cast to an appropriate end point depending on the 
        ///     connection type.
        /// </remarks>
        public IPEndPoint EndPoint { get; protected set; }

        public IPMode IPMode { get; protected set; }

        /// <summary>
        ///     The traffic statistics about this Connection.
        /// </summary>
        /// <remarks>
        ///     Contains statistics about the number of messages and bytes sent and received by this connection.
        /// </remarks>
        public ConnectionStatistics Statistics { get; protected set; }

        /// <summary>
        ///     The state of this connection.
        /// </summary>
        /// <remarks>
        ///     All implementers should be aware that when this is set to ConnectionState.Connected it will
        ///     release all threads that are blocked on <see cref="WaitOnConnect"/>.
        /// </remarks>
        public ConnectionState State
        {
            get
            {
                return this._state;
            }
            
            protected set
            {
                this._state = value;
                this.SetState(value);
            }
        }

        protected ConnectionState _state;
        protected virtual void SetState(ConnectionState state) { }
        
        /// <summary>
        ///     Constructor that initializes the ConnecitonStatistics object.
        /// </summary>
        /// <remarks>
        ///     This constructor initialises <see cref="Statistics"/> with empty statistics and sets <see cref="State"/> to 
        ///     <see cref="ConnectionState.NotConnected"/>.
        /// </remarks>
        protected Connection()
        {
            this.Statistics = new ConnectionStatistics();
            this.State = ConnectionState.NotConnected;
        }

        /// <summary>
        ///     Sends a number of bytes to the end point of the connection using the specified <see cref="MessageType"/>.
        /// </summary>
        /// <param name="msg">The message to send.</param>
        /// <remarks>
        ///     <include file="DocInclude/common.xml" path="docs/item[@name='Connection_SendBytes_General']/*" />
        ///     <para>
        ///         The messageType parameter is only a request to use those options and the actual method used to send the
        ///         data is up to the implementation. There are circumstances where this parameter may be ignored but in 
        ///         general any implementer should aim to always follow the user's request.
        ///     </para>
        /// </remarks>
        public abstract ValueTask SendAsync(IMessageWriter msg);

        /// <summary>
        ///     Sends a number of bytes to the end point of the connection using the specified <see cref="MessageType"/>.
        /// </summary>
        /// <param name="bytes">The bytes of the message to send.</param>
        /// <param name="messageType">The option specifying how the message should be sent.</param>
        /// <remarks>
        ///     <include file="DocInclude/common.xml" path="docs/item[@name='Connection_SendBytes_General']/*" />
        ///     <para>
        ///         The messageType parameter is only a request to use those options and the actual method used to send the
        ///         data is up to the implementation. There are circumstances where this parameter may be ignored but in 
        ///         general any implementer should aim to always follow the user's request.
        ///     </para>
        /// </remarks>
        public abstract ValueTask SendBytes(byte[] bytes, MessageType messageType = MessageType.Unreliable);

        /// <summary>
        ///     Connects the connection to a server and begins listening.
        ///     This method does not block.
        /// </summary>
        /// <param name="bytes">The bytes of data to send in the handshake.</param>
        public abstract ValueTask ConnectAsync(byte[] bytes = null);

        /// <summary>
        ///     Invokes the DataReceived event.
        /// </summary>
        /// <param name="msg">The bytes received.</param>
        /// <param name="messageType">The <see cref="MessageType"/> the message was received with.</param>
        /// <remarks>
        ///     Invokes the <see cref="DataReceived"/> event on this connection to alert subscribers a new message has been
        ///     received. The bytes and the send option that the message was sent with should be passed in to give to the
        ///     subscribers.
        /// </remarks>
        protected async ValueTask InvokeDataReceived(IMessageReader msg, MessageType messageType)
        {
            // Make a copy to avoid race condition between null check and invocation
            var handler = DataReceived;
            if (handler != null)
            {
                try
                {
                    await handler(new DataReceivedEventArgs(this, msg, messageType));
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Invoking data received failed");
                    await Disconnect("Invoking data received failed");
                }
            }
        }

        /// <summary>
        ///     Invokes the Disconnected event.
        /// </summary>
        /// <param name="e">The exception, if any, that occurred to cause this.</param>
        /// <param name="reader">Extra disconnect data</param>
        /// <remarks>
        ///     Invokes the <see cref="Disconnected"/> event to alert subscribres this connection has been disconnected either 
        ///     by the end point or because an error occurred. If an error occurred the error should be passed in in order to 
        ///     pass to the subscribers, otherwise null can be passed in.
        /// </remarks>
        protected async ValueTask InvokeDisconnected(string e, IMessageReader reader)
        {
            // Make a copy to avoid race condition between null check and invocation
            var handler = Disconnected;
            if (handler != null)
            {
                try
                {
                    await handler(new DisconnectedEventArgs(e, reader));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error in InvokeDisconnected");
                }
            }
        }

        /// <summary>
        /// For times when you want to force the disconnect handler to fire as well as close it.
        /// If you only want to close it, just use Dispose.
        /// </summary>
        public abstract ValueTask Disconnect(string reason, MessageWriter writer = null);
        
        /// <summary>
        ///     Disposes of this NetworkConnection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes of this NetworkConnection.
        /// </summary>
        /// <param name="disposing">Are we currently disposing?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DataReceived = null;
                this.Disconnected = null;
            }
        }
    }
}
