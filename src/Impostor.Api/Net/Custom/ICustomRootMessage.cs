using System.Threading.Tasks;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Net.Custom
{
    public interface ICustomRootMessage : ICustomMessage
    {
        ValueTask HandleMessageAsync(IClient client, IMessageReader reader, MessageType messageType);
    }
}
