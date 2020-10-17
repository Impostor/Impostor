using System;
using System.Net;
using System.Threading.Tasks;
using Impostor.Server.Games;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Net
{
    /// <summary>
    ///     Represents the connection of the client.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        ///     Gets the IP endpoint of the client.
        /// </summary>
        IPEndPoint EndPoint { get; }

        /// <summary>
        ///     Gets a value indicating whether the client is connected to the server.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        ///     Gets or sets the client of the connection.
        /// </summary>
        IClient? Client { get; set; }

        /// <summary>
        ///     Create a message writer that can be send to the connection.
        /// </summary>
        /// <remarks>
        ///     Be aware when implementing a custom connection handler that this method is not called when a message
        ///     is being send in <see cref="IGame"/>.
        /// </remarks>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>Message writer for the current connection.</returns>
        IConnectionMessageWriter CreateMessage(MessageType messageType);
    }
}