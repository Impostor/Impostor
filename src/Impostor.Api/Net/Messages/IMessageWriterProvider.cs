namespace Impostor.Api.Net.Messages
{
    public interface IMessageWriterProvider
    {
        /// <summary>
        ///     Retrieves a <see cref="IMessageWriter" /> from the internal pool.
        ///     Make sure to call <see cref="System.IDisposable.Dispose" /> when you are done!.
        /// </summary>
        /// <param name="sendOption">
        ///     Whether to send the message as <see cref="MessageType.Reliable" /> or <see cref="MessageType.Unreliable" />.
        ///     Reliable packets will ensure delivery while unreliable packets may be lost.
        /// </param>
        /// <returns>A <see cref="IMessageWriter" /> from the pool.</returns>
        IMessageWriter Get(MessageType sendOption = MessageType.Unreliable);
    }
}
