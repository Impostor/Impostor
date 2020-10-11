using System.Threading.Tasks;

namespace Impostor.Server.Net.Messages
{
    /// <summary>
    ///     Represents the message writer for <see cref="IConnection"/>.
    /// </summary>
    public interface IConnectionMessageWriter : IMessageWriter
    {
        /// <summary>
        ///     Gets the connection where the message writer belongs to.
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        ///     Sends the message to the <see cref="Connection"/>.
        /// </summary>
        /// <returns>Task.</returns>
        ValueTask SendAsync();
    }
}