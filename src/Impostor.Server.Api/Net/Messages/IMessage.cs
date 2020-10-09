namespace Impostor.Server.Net
{
    public interface IMessage
    {
        MessageType Type { get; }
        
        IMessageReader CreateReader();
    }
}