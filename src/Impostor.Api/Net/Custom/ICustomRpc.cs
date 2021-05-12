using System.Threading.Tasks;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Net.Custom
{
    public interface ICustomRpc : ICustomMessage
    {
        ValueTask<bool> HandleRpcAsync(IInnerNetObject innerNetObject, IClientPlayer sender, IClientPlayer? target, IMessageReader reader);
    }
}
