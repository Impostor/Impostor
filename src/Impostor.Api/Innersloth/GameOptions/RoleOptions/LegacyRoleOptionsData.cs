using System.Collections.Generic;

namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class LegacyRoleOptionsData
{
    public bool ShapeshifterLeaveSkin { get; set; }

    public byte ShapeshifterCooldown { get; set; } = 10;

    public byte ShapeshifterDuration { get; set; } = 30;

    public byte ScientistCooldown { get; set; } = 15;

    public byte ScientistBatteryCharge { get; set; } = 5;

    public byte GuardianAngelCooldown { get; set; } = 60;

    public bool ImpostorsCanSeeProtect { get; set; }

    public byte ProtectionDurationSeconds { get; set; } = 10;

    public byte EngineerCooldown { get; set; } = 30;

    public byte EngineerInVentMaxTime { get; set; } = 15;

    public Dictionary<RoleTypes, RoleRate> RoleRates { get; } = new Dictionary<RoleTypes, RoleRate>();

    public static LegacyRoleOptionsData Deserialize(IMessageReader reader)
    {
        var roleOptionsData = new LegacyRoleOptionsData();
        var num = reader.ReadPackedInt32();
        for (var i = 0; i < num; i++)
        {
            var key = (RoleTypes)reader.ReadInt16();
            var roleRate = RoleRate.Deserialize(reader);
            roleOptionsData.RoleRates[key] = roleRate;
        }

        roleOptionsData.ShapeshifterLeaveSkin = reader.ReadBoolean();
        roleOptionsData.ShapeshifterCooldown = reader.ReadByte();
        roleOptionsData.ShapeshifterDuration = reader.ReadByte();
        roleOptionsData.ScientistCooldown = reader.ReadByte();
        roleOptionsData.GuardianAngelCooldown = reader.ReadByte();
        roleOptionsData.EngineerCooldown = reader.ReadByte();
        roleOptionsData.EngineerInVentMaxTime = reader.ReadByte();
        roleOptionsData.ScientistBatteryCharge = reader.ReadByte();
        roleOptionsData.ProtectionDurationSeconds = reader.ReadByte();
        roleOptionsData.ImpostorsCanSeeProtect = reader.ReadBoolean();
        return roleOptionsData;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.WritePacked(RoleRates.Count);
        foreach (var roleRate in RoleRates)
        {
            writer.Write((ushort)roleRate.Key);
            roleRate.Value.Serialize(writer);
        }

        writer.Write(ShapeshifterLeaveSkin);
        writer.Write(ShapeshifterCooldown);
        writer.Write(ShapeshifterDuration);
        writer.Write(ScientistCooldown);
        writer.Write(GuardianAngelCooldown);
        writer.Write(EngineerCooldown);
        writer.Write(EngineerInVentMaxTime);
        writer.Write(ScientistBatteryCharge);
        writer.Write(ProtectionDurationSeconds);
        writer.Write(ImpostorsCanSeeProtect);
    }
}
