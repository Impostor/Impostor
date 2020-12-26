using Impostor.Api.Events.Managers;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class SecurityCameraSystemType : ISystemType
    {
        public byte InUse { get; internal set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(IMessageReader reader, bool initialState, IEventManager eventManager)
        {
            InUse = reader.ReadByte();
        }
    }
}