using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class ElectricalDoors : ISystemType
    {
        private uint _state;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.Write(_state);
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            _state = reader.ReadUInt32();
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
        }
    }
}
