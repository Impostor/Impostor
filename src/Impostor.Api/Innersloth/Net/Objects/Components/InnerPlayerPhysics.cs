using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects.Components
{
    public class InnerPlayerPhysics : InnerNetObject
    {
        public override void HandleRpc(IClientPlayer sender, IClientPlayer target, RpcCalls call, IMessageReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IMessageReader reader, bool initialState)
        {
            throw new System.NotImplementedException();
        }
    }
}