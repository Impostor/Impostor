using System;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;
using Serilog;

namespace Impostor.Hazel
{
    /// <summary>
    ///     Base class for all connection listeners.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         ConnectionListeners are server side objects that listen for clients and create matching server side connections 
    ///         for each client in a similar way to TCP does. These connections should be ready for communication immediately.
    ///     </para>
    ///     <para>
    ///         Each time a client connects the <see cref="NewConnection"/> event will be invoked to alert all subscribers to
    ///         the new connection. A disconnected event is then present on the <see cref="Connection"/> that is passed to the
    ///         subscribers.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true"/>
    public abstract class ConnectionListener : IAsyncDisposable
    {
        private static readonly ILogger Logger = Log.ForContext<ConnectionListener>();

        /// <summary>
        ///     Invoked when a new client connects.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         NewConnection is invoked each time a client connects to the listener. The 
        ///         <see cref="NewConnectionEventArgs"/> contains the new <see cref="Connection"/> for communication with this
        ///         client.
        ///     </para>
        ///     <para>
        ///         Hazel may or may not store connections so it is your responsibility to keep track and properly Dispose of 
        ///         connections to your server. 
        ///     </para>
        ///     <include file="DocInclude/common.xml" path="docs/item[@name='Event_Thread_Safety_Warning']/*" />
        /// </remarks>
        /// <example>
        ///     <code language="C#" source="DocInclude/TcpListenerExample.cs"/>
        /// </example>
        public Func<NewConnectionEventArgs, ValueTask> NewConnection;

        /// <summary>
        ///     Makes this connection listener begin listening for connections.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This instructs the listener to begin listening for new clients connecting to the server. When a new client 
        ///         connects the <see cref="NewConnection"/> event will be invoked containing the connection to the new client.
        ///     </para>
        ///     <para>
        ///         To stop listening you should call <see cref="DisposeAsync()"/>.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <code language="C#" source="DocInclude/TcpListenerExample.cs"/>
        /// </example>
        public abstract Task StartAsync();

        /// <summary>
        ///     Invokes the NewConnection event with the supplied connection.
        /// </summary>
        /// <param name="msg">The user sent bytes that were received as part of the handshake.</param>
        /// <param name="connection">The connection to pass in the arguments.</param>
        /// <remarks>
        ///     Implementers should call this to invoke the <see cref="NewConnection"/> event before data is received so that
        ///     subscribers do not miss any data that may have been sent immediately after connecting.
        /// </remarks>
        internal async Task InvokeNewConnection(IMessageReader msg, Connection connection)
        {
            // Make a copy to avoid race condition between null check and invocation
            var handler = NewConnection;
            if (handler != null)
            {
                try
                {
                    await handler(new NewConnectionEventArgs(msg, connection));
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Accepting connection failed");
                    await connection.Disconnect("Accepting connection failed");
                }
            }
        }

        /// <summary>
        ///     Call to dispose of the connection listener.
        /// </summary>
        public virtual ValueTask DisposeAsync()
        {
            this.NewConnection = null;
            return ValueTask.CompletedTask;
        }
    }
}
