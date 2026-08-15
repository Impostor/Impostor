using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class HudOverrideSystemType : ISystemType, IActivatable
    {
        public bool IsActive { get; private set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.Write(IsActive);
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            IsActive = reader.ReadBoolean();
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            UpdateSystem(playerControl, reader.ReadByte());
        }

        internal void UpdateSystem(IInnerPlayerControl? playerControl, byte amount)
        {
            IsActive = (amount & 0x80) != 0;
        }
    }
}
