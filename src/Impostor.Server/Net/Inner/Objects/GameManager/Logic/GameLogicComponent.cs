using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.GameManager.Logic;

internal abstract class GameLogicComponent
{
    public virtual void OnPlayerDisconnect(IInnerPlayerControl pc)
    {
    }

    public virtual void HandleRPC(RpcCalls callId, IMessageReader reader)
    {
    }

    public abstract bool Serialize(IMessageWriter writer, bool initialState);

    public abstract void Deserialize(IMessageReader reader, bool initialState);
}
