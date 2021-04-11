using System.Net;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Net
{
    /// <summary>
    ///     Represents the connection of the client.
    /// </summary>
    public interface IHazelConnection
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
        ///     Gets the average ping of the client.
        /// </summary>
        float AveragePing { get; }

        /// <summary>
        ///     Sends a message writer to the connection.
        /// </summary>
        /// <param name="writer">The message.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask SendAsync(IMessageWriter writer);

        /// <summary>
        ///     Disconnects the client and invokes the disconnect handler.
        /// </summary>
        /// <param name="reason">A reason.</param>
        /// <param name="writer">A message to send with disconnect packet.</param>
        /// <returns>Task that must be awaited.</returns>
        ValueTask DisconnectAsync(string? reason, IMessageWriter? writer = null);
    }
}
