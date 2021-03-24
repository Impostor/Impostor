using Impostor.Api.Net.Messages;

namespace Impostor.Hazel
{
    public struct DataReceivedEventArgs
    {
        public readonly Connection Sender;

        /// <summary>
        ///     The bytes received from the client.
        /// </summary>
        public readonly IMessageReader Message;

        /// <summary>
        ///     The <see cref="Type"/> the data was sent with.
        /// </summary>
        public readonly MessageType Type;
        
        public DataReceivedEventArgs(Connection sender, IMessageReader msg, MessageType type)
        {
            this.Sender = sender;
            this.Message = msg;
            this.Type = type;
        }
    }
}
