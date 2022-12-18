using System.Threading.Tasks;
using Impostor.Api.Net.Inner;

namespace Impostor.Api.Net.Custom
{
    public interface ICustomRpc : ICustomMessage
    {
        ValueTask<bool> HandleRpcAsync(IInnerNetObject innerNetObject, IClientPlayer sender, IClientPlayer? target, IMessageReader reader);
    }
}
