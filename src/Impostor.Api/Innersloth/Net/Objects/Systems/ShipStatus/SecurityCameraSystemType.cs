using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects.Systems.ShipStatus
{
    public class SecurityCameraSystemType : ISystemType
    {
        public byte InUse { get; internal set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            InUse = reader.ReadByte();
        }
    }
}