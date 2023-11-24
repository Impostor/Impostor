using System.Collections.Generic;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;

public class MushroomMixupSabotageSystemType : ISystemType
{
    private readonly Dictionary<byte, CondensedOutfit> _currentMixups = new();
    private State _currentState;
    private float _currentSecondsUntilHeal;

    private enum State
    {
        Inactive,
        JustTriggered,
        IdleButMixedUp,
    }

    public void Serialize(IMessageWriter writer, bool initialState)
    {
        writer.Write((byte)_currentState);
        writer.Write(_currentSecondsUntilHeal);

        writer.WritePacked(_currentMixups.Count);
        foreach (var (playerId, outfit) in _currentMixups)
        {
            writer.Write(playerId);
            outfit.Serialize(writer);
        }
    }

    public void Deserialize(IMessageReader reader, bool initialState)
    {
        _currentState = (State)reader.ReadByte();
        _currentSecondsUntilHeal = reader.ReadSingle();

        _currentMixups.Clear();
        var num = reader.ReadByte();
        for (var i = 0; i < num; i++)
        {
            var playerId = reader.ReadByte();
            var outfit = CondensedOutfit.Deserialize(reader);

            _currentMixups.Add(playerId, outfit);
        }
    }

    private readonly record struct CondensedOutfit(
        byte HatIndex,
        byte VisorIndex,
        byte SkinIndex,
        byte PetIndex,
        byte ColorPlayerId
    )
    {
        public void Serialize(IMessageWriter writer)
        {
            writer.Write(ColorPlayerId);
            writer.Write(HatIndex);
            writer.Write(VisorIndex);
            writer.Write(SkinIndex);
            writer.Write(PetIndex);
        }

        public static CondensedOutfit Deserialize(IMessageReader reader)
        {
            return new CondensedOutfit
            {
                ColorPlayerId = reader.ReadByte(),
                HatIndex = reader.ReadByte(),
                VisorIndex = reader.ReadByte(),
                SkinIndex = reader.ReadByte(),
                PetIndex = reader.ReadByte(),
            };
        }
    }
}
