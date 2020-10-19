using System.Net;

namespace Impostor.Hazel
{
    /// <summary>
    ///     Abstract base class for a <see cref="ConnectionListener"/> for network based connections.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public abstract class NetworkConnectionListener : ConnectionListener
    {
        /// <summary>
        ///     The local end point the listener is listening for new clients on.
        /// </summary>
        public IPEndPoint EndPoint { get; protected set; }

        /// <summary>
        ///     The <see cref="IPMode">IPMode</see> the listener is listening for new clients on.
        /// </summary>
        public IPMode IPMode { get; protected set; }
    }
}
