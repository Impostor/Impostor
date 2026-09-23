using System.Collections.Generic;
using System.Linq;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Maps;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class AutoDoorsSystemType : ISystemType, IDoorSystem
    {
        private readonly Dictionary<int, bool> _doors;
        private readonly IReadOnlyDictionary<int, DoorData> _doorData;
        private uint _dirtyBits;

        public AutoDoorsSystemType(Dictionary<int, bool> doors, IReadOnlyDictionary<int, DoorData> doorData)
        {
            _doors = doors;
            _doorData = doorData;
        }

        public bool IsActive => _doors.Values.Any(open => !open);

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            if (initialState)
            {
                for (var i = 0; i < _doors.Count; i++)
                {
                    writer.Write(_doors[i]);
                }

                return;
            }

            writer.WritePacked(_dirtyBits);
            for (var i = 0; i < _doors.Count; i++)
            {
                if ((_dirtyBits & (uint)(1 << i)) != 0)
                {
                    writer.Write(_doors[i]);
                }
            }
        }

        public void Deserialize(IMessageReader reader, bool initialState)
        {
            if (initialState)
            {
                for (var i = 0; i < _doors.Count; i++)
                {
                    _doors[i] = reader.ReadBoolean();
                }
            }
            else
            {
                var num = reader.ReadPackedUInt32();

                for (var i = 0; i < _doors.Count; i++)
                {
                    if ((num & (uint)(1 << i)) != 0)
                    {
                        _doors[i] = reader.ReadBoolean();
                    }
                }
            }
        }

        public void UpdateSystem(IInnerPlayerControl? playerControl, IMessageReader reader)
        {
        }

        public void CloseDoorsOfType(SystemTypes room)
        {
            for (var i = 0; i < _doors.Count; i++)
            {
                if (_doorData.TryGetValue(i, out var door) && door.Room == room)
                {
                    _doors[i] = false;
                    _dirtyBits |= (uint)(1 << i);
                }
            }
        }
    }
}
