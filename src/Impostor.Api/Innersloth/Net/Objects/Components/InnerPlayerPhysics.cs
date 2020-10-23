using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects.Components
{
    public class InnerPlayerPhysics : InnerNetObject
    {
        public override void HandleRpc(byte callId, IMessageReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(IMessageReader reader, bool initialState)
        {
            throw new System.NotImplementedException();
        }
    }
}