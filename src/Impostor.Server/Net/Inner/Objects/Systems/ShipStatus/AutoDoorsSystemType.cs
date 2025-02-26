using System;
using System.Collections.Generic;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;

public class AutoDoorsSystemType(Dictionary<int, bool> doors) : ISystemType
{
    public void Serialize(IMessageWriter writer, bool initialState)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(IMessageReader reader, bool initialState)
    {
        if (initialState)
        {
            for (var i = 0; i < doors.Count; i++)
            {
                doors[i] = reader.ReadBoolean();
            }
        }
        else
        {
            var num = reader.ReadPackedUInt32();

            for (var i = 0; i < doors.Count; i++)
            {
                if ((num & (1 << i)) != 0)
                {
                    doors[i] = reader.ReadBoolean();
                }
            }
        }
    }
}
