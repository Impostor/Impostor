using System;
using System.Collections.Generic;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;

public class ElectricalDoors(Dictionary<int, bool> doors) : ISystemType
{
    public void Serialize(IMessageWriter writer, bool initialState)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(IMessageReader reader, bool initialState)
    {
        var num = reader.ReadUInt32();
        for (var i = 0; i < doors.Count; i++)
        {
            doors[i] = (num & (ulong)(1L << (i & 31))) > 0UL;
        }
    }
}
