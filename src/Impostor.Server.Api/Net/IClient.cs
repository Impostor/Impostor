using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.Net
{
    /// <summary>
    ///     Represents a connected game client.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        ///     Gets or sets the unique ID of the client.
        /// </summary>
        /// <remarks>
        ///     This ID is generated when the client is registered in the client manager and should not be used
        ///     to store persisted data.
        /// </remarks>
        int Id { get; set; }

        /// <summary>
        ///     Gets the name that was provided by the player in the client.
        /// </summary>
        /// <remarks>
        ///     The name is provided by the player and should not be used to store persisted data.
        /// </remarks>
        string Name { get; }

        /// <summary>
        ///     Gets the connection of the client.
        /// </summary>
        /// <remarks>
        ///     Null when the client was not registered by the matchmaker.
        /// </remarks>
        IConnection? Connection { get; }

        /// <summary>
        ///     Gets a value indicating whether the client is a bot.
        /// </summary>
        bool IsBot { get; }

        /// <summary>
        ///     Gets a key/value collection that can be used to share data between messages.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The stored data will not be saved.
        ///         After the connection has been closed all data will be lost.
        ///     </para>
        ///     <para>
        ///         Note that the values will not be disposed after the connection has been closed.
        ///         This has to be implemented by the plugin.
        ///     </para>
        /// </remarks>
        IDictionary<object, object> Items { get; }

        /// <summary>
        ///     Gets or sets the current game data of the <see cref="IClient"/>.
        /// </summary>
        IClientPlayer? Player { get; set; }

        ValueTask HandleMessageAsync(IMessage message);

        ValueTask HandleDisconnectAsync(string reason);
    }
}