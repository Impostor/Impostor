using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class MovingPlatformBehaviour : ISystemType
    {
        private byte _useId;

        public uint TargetNetId { get; private set; } = uint.MaxValue;

        public bool IsLeft { get; private set; } = true;

        public bool InUse => TargetNetId != uint.MaxValue;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            _useId++;
            writer.Write(_useId);
            writer.Write(TargetNetId);
            writer.Write(IsLeft);
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            if (initialState)
            {
                _useId = reader.ReadByte();
                TargetNetId = reader.ReadUInt32();
                IsLeft = reader.ReadBoolean();
                return;
            }

            var newSid = reader.ReadByte();
            if (SidGreaterThan(newSid, _useId))
            {
                _useId = newSid;
                TargetNetId = reader.ReadUInt32();
                IsLeft = reader.ReadBoolean();
            }
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
        }

        private static bool SidGreaterThan(byte newSid, byte prevSid)
        {
            var num = (byte)(prevSid + sbyte.MaxValue);

            return prevSid < num
                ? newSid > prevSid && newSid <= num
                : newSid > prevSid || newSid <= num;
        }
    }
}
