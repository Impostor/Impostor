namespace Impostor.Api.Net.Messages
{
    public interface IMessage
    {
        MessageType Type { get; }

        IMessageReader CreateReader();
    }
}