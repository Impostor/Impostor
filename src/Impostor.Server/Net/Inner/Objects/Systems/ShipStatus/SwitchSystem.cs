using System;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class SwitchSystem : ISystemType, IActivatable
    {
        public byte ExpectedSwitches { get; set; }

        public byte ActualSwitches { get; set; }

        public byte Value { get; set; } = byte.MaxValue;

        public bool IsActive { get; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            ExpectedSwitches = reader.ReadByte();
            ActualSwitches = reader.ReadByte();
            Value = reader.ReadByte();
        }
    }
}
