using System.Collections.Generic;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Maps;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class DoorsSystemType : ISystemType, IDoorSystem
    {
        private readonly Dictionary<SystemTypes, float> _timers = new();
        private readonly Dictionary<int, bool> _doors;
        private readonly IReadOnlyDictionary<int, DoorData> _doorData;

        public DoorsSystemType(Dictionary<int, bool> doors, IReadOnlyDictionary<int, DoorData> doorData)
        {
            _doors = doors;
            _doorData = doorData;
        }

        public bool IsActive => false;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            writer.Write((byte)_timers.Count);
            foreach (var timer in _timers)
            {
                writer.Write((byte)timer.Key);
                writer.Write(timer.Value);
            }

            for (var i = 0; i < _doors.Count; i++)
            {
                writer.Write(_doors[i]);
            }
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            var num = reader.ReadByte();
            for (var i = 0; i < num; i++)
            {
                var systemType = (SystemTypes)reader.ReadByte();
                var value = reader.ReadSingle();

                _timers[systemType] = value;
            }

            for (var j = 0; j < _doors.Count; j++)
            {
                _doors[j] = reader.ReadBoolean();
            }
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
            var value = reader.ReadByte();
            var doorId = value & 0x1f;
            if ((value & 0xc0) == 0x40 && _doors.ContainsKey(doorId))
            {
                _doors[doorId] = true;
            }
        }

        public void CloseDoorsOfType(SystemTypes room)
        {
            _timers[room] = 30f;

            for (var i = 0; i < _doors.Count; i++)
            {
                if (_doorData.TryGetValue(i, out var door) && door.Room == room)
                {
                    _doors[i] = false;
                }
            }
        }
    }
}
