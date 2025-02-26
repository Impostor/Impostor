using System;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;

public class SabotageSystemType(IActivatable[] specials) : ISystemType
{
    private readonly IActivatable[] _specials = specials;

    public float Timer { get; set; }

    public void Serialize(IMessageWriter writer, bool initialState)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(IMessageReader reader, bool initialState)
    {
        Timer = reader.ReadSingle();
    }
}
