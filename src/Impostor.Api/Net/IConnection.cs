using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Net
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
        ///     Gets the client of the connection.
        /// </summary>
        IClient? Client { get; }

        /// <summary>
        ///     Sends a message writer to the connection.
        /// </summary>
        /// <param name="writer">The message.</param>
        /// <returns></returns>
        ValueTask SendAsync(IMessageWriter writer);
    }
}