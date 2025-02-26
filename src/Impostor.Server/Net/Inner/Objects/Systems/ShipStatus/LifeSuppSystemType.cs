using System;
using System.Collections.Generic;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;

public class LifeSuppSystemType : ISystemType, IActivatable
{
    public float Countdown { get; private set; } = 10000f;

    public HashSet<int> CompletedConsoles { get; } = new();

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

        if (reader.Position >= reader.Length)
        {
            return;
        }

        CompletedConsoles.Clear(); // TODO: Thread safety

        var num = reader.ReadPackedInt32();

        for (var i = 0; i < num; i++)
        {
            CompletedConsoles.Add(reader.ReadPackedInt32());
        }
    }
}
