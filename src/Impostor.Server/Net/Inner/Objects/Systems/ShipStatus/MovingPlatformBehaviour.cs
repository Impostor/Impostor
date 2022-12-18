using System;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class MovingPlatformBehaviour : ISystemType, IActivatable
    {
        public bool IsActive { get; private set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            var sid = reader.ReadByte();
            var targetId = reader.ReadUInt32();
            var isLeft = reader.ReadBoolean();
        }
    }
}
