using Impostor.Hazel;

namespace Impostor.Server.Net.Messages
{
    public class MessageWriterProvider : IMessageWriterProvider
    {
        public IMessageWriter Get(MessageType sendOption = MessageType.Unreliable)
        {
            return MessageWriter.Get(sendOption);
        }
    }
}
