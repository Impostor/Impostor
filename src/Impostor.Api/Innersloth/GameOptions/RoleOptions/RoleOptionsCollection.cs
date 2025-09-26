using System;
using System.Collections.Generic;

namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class RoleOptionsCollection
{
    public RoleOptionsCollection(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public Dictionary<RoleTypes, RoleData> Roles { get; } = new Dictionary<RoleTypes, RoleData>();

    public void Deserialize(IMessageReader reader)
    {
        var count = reader.ReadPackedInt32();
        Roles.EnsureCapacity(count);
        for (var i = 0; i < count; i++)
        {
            var roleType = (RoleTypes)reader.ReadInt16();
            var roleRate = RoleRate.Deserialize(reader);
            var roleOptionsReader = reader.ReadMessage();
            IRoleOptions roleOptions = roleType switch
            {
                RoleTypes.Scientist => ScientistRoleOptions.Deserialize(roleOptionsReader, Version),
                RoleTypes.Engineer => EngineerRoleOptions.Deserialize(roleOptionsReader, Version),
                RoleTypes.GuardianAngel => GuardianAngelRoleOptions.Deserialize(roleOptionsReader, Version),
                RoleTypes.Shapeshifter => ShapeshifterRoleOptions.Deserialize(roleOptionsReader, Version),
                RoleTypes.Noisemaker => NoisemakerRoleOptions.Deserialize(roleOptionsReader, Version),
                RoleTypes.Phantom => PhantomRoleOptions.Deserialize(roleOptionsReader, Version),
                RoleTypes.Tracker => TrackerRoleOptions.Deserialize(roleOptionsReader, Version),
                RoleTypes.Detective => DetectiveRoleOptions.Deserialize(roleOptionsReader, Version),
                RoleTypes.Viper => ViperRoleOptions.Deserialize(roleOptionsReader, Version),
                _ => throw new ArgumentOutOfRangeException(nameof(roleType), roleType, null),
            };

            Roles[roleType] = new RoleData(roleType, roleOptions, roleRate);
        }
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.WritePacked(Roles.Count);
        foreach (var (key, roleData) in Roles)
        {
            writer.Write((ushort)key);
            roleData.Rate.Serialize(writer);
            writer.StartMessage(0);
            roleData.RoleOptions.Serialize(writer);
            writer.EndMessage();
        }
    }

    public readonly record struct RoleData(RoleTypes Type, IRoleOptions RoleOptions, RoleRate Rate);
}
