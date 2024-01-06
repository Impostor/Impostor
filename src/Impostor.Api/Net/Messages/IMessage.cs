namespace Impostor.Api.Net.Messages;

public interface IMessage
{
    public void Serialize<T>(T messageData) where T : IMessageData;

    public void Deserialize<T>(T messageData) where T : IMessageData;

    public T Get<T>();
}
