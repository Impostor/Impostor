using System;
using System.Collections.Generic;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class AutoDoorsSystemType : ISystemType
    {
        private readonly Dictionary<int, bool> _doors;

        public AutoDoorsSystemType(Dictionary<int, bool> doors)
        {
            _doors = doors;
        }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
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
                    if ((num & 1 << i) != 0)
                    {
                        _doors[i] = reader.ReadBoolean();
                    }
                }
            }
        }
    }
}
