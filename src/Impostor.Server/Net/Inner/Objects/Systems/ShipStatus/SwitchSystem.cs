using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class SwitchSystem : ISystemType, IActivatable
    {
        public byte ExpectedSwitches { get; set; }

        public byte ActualSwitches { get; set; }

        public byte Value { get; set; } = byte.MaxValue;

        public bool IsActive => ExpectedSwitches != ActualSwitches;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.Write(ExpectedSwitches);
            writer.Write(ActualSwitches);
            writer.Write(Value);
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            ExpectedSwitches = reader.ReadByte();
            ActualSwitches = reader.ReadByte();
            Value = reader.ReadByte();
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            UpdateSystem(playerControl, reader.ReadByte());
        }

        internal void UpdateSystem(IInnerPlayerControl? playerControl, byte amount)
        {
            var value = amount;
            if ((value & 0x80) != 0)
            {
                ActualSwitches ^= (byte)(value & 0x1f);
            }
            else
            {
                ActualSwitches ^= (byte)(1 << value);
            }
        }
    }
}
