using System;
using System.Threading.Tasks;
using Impostor.Api.Net.Inner;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic;

internal abstract class GameLogicComponent
{
    public virtual ValueTask<bool> HandleRpcAsync(RpcCalls callId, IMessageReader reader)
    {
        return ValueTask.FromResult(false);
    }

    public virtual ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
    {
        throw new NotImplementedException();
    }

    public virtual ValueTask DeserializeAsync(IMessageReader reader, bool initialState)
    {
        return default;
    }
}
