using System;
using System.Collections.Generic;
using Impostor.Api.Innersloth;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class DoorsSystemType : ISystemType
    {
        private readonly Dictionary<SystemTypes, float> _timers = new Dictionary<SystemTypes, float>();
        private readonly Dictionary<int, bool> _doors;

        public DoorsSystemType(Dictionary<int, bool> doors)
        {
            _doors = doors;
        }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
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
    }
}
