using System.Threading.Tasks;

namespace Impostor.Api.Net.Custom
{
    public interface ICustomRootMessage : ICustomMessage
    {
        ValueTask HandleMessageAsync(IClient client, IMessageReader reader, MessageType messageType);
    }
}
