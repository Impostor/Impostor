using System;
using System.Collections.Generic;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;

public class ReactorSystemType : ISystemType, IActivatable
{
    public float Countdown { get; private set; } = 10000f;

    public HashSet<Tuple<byte, byte>> UserConsolePairs { get; } = new();

    public bool IsActive
    {
        get => Countdown < 10000.0;
    }

    public void Serialize(IMessageWriter writer, bool initialState)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(IMessageReader reader, bool initialState)
    {
        Countdown = reader.ReadSingle();
        UserConsolePairs.Clear(); // TODO: Thread safety

        var count = reader.ReadPackedInt32();

        for (var i = 0; i < count; i++)
        {
            UserConsolePairs.Add(new Tuple<byte, byte>(reader.ReadByte(), reader.ReadByte()));
        }
    }
}
