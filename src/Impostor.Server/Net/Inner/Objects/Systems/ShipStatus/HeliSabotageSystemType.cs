using System;
using System.Collections.Generic;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;

public class HeliSabotageSystemType : ISystemType, IActivatable
{
    public float Countdown { get; private set; } = 10000f;

    public float Timer { get; private set; }

    public HashSet<Tuple<byte, byte>> ActiveConsoles { get; } = new();

    public HashSet<byte> CompletedConsoles { get; } = new();

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
        Timer = reader.ReadSingle();
        ActiveConsoles.Clear(); // TODO: Thread safety
        CompletedConsoles.Clear(); // TODO: Thread safety

        var activeCount = reader.ReadPackedUInt32();

        for (var i = 0; i < activeCount; i++)
        {
            ActiveConsoles.Add(new Tuple<byte, byte>(reader.ReadByte(), reader.ReadByte()));
        }

        var completedCount = reader.ReadPackedUInt32();

        for (var i = 0; i < completedCount; i++)
        {
            CompletedConsoles.Add(reader.ReadByte());
        }
    }
}
