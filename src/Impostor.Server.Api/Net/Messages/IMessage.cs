namespace Impostor.Server.Net.Messages
{
    public interface IMessage
    {
        MessageType Type { get; }

        IMessageReader CreateReader();
    }
}