using System;
using System.Collections.Generic;
using System.IO;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth
{
    public class RoleOptionsData
    {
        public bool ShapeshifterLeaveSkin { get; set; }

        public float ShapeshifterCooldown { get; set; } = 10f;

        public float ShapeshifterDuration { get; set; } = 30f;

        public float ScientistCooldown { get; set; } = 15f;

        public float ScientistBatteryCharge { get; set; } = 5f;

        public float GuardianAngelCooldown { get; set; } = 60f;

        public bool ImpostorsCanSeeProtect { get; set; }

        public float ProtectionDurationSeconds { get; set; } = 10f;

        public float EngineerCooldown { get; set; } = 30f;

        public float EngineerInVentMaxTime { get; set; } = 15f;

        public Dictionary<RoleTypes, RoleRate> RoleRates { get; } = new Dictionary<RoleTypes, RoleRate>();

        public static RoleOptionsData Deserialize(ReadOnlySpan<byte> span)
        {
            var roleOptionsData = new RoleOptionsData();
            var num = span.ReadInt32();
            for (var i = 0; i < num; i++)
            {
                var key = (RoleTypes)span.ReadInt16();
                var roleRate = new RoleRate(span.ReadByte(), span.ReadByte());
                roleOptionsData.RoleRates[key] = roleRate;
            }

            roleOptionsData.ShapeshifterLeaveSkin = span.ReadBoolean();
            roleOptionsData.ShapeshifterCooldown = span.ReadByte();
            roleOptionsData.ShapeshifterDuration = span.ReadByte();
            roleOptionsData.ScientistCooldown = span.ReadByte();
            roleOptionsData.GuardianAngelCooldown = span.ReadByte();
            roleOptionsData.EngineerCooldown = span.ReadByte();
            roleOptionsData.EngineerInVentMaxTime = span.ReadByte();
            roleOptionsData.ScientistBatteryCharge = span.ReadByte();
            roleOptionsData.ProtectionDurationSeconds = span.ReadByte();
            roleOptionsData.ImpostorsCanSeeProtect = span.ReadBoolean();
            return roleOptionsData;
        }

        public static RoleOptionsData Deserialize(IMessageReader reader)
        {
            var roleOptionsData = new RoleOptionsData();
            var num = reader.ReadPackedInt32();
            for (var i = 0; i < num; i++)
            {
                var key = (RoleTypes)reader.ReadInt16();
                var roleRate = new RoleRate(reader.ReadByte(), reader.ReadByte());
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
                writer.Write((byte)roleRate.Value.MaxCount);
                writer.Write((byte)roleRate.Value.Chance);
            }

            writer.Write(ShapeshifterLeaveSkin);
            writer.Write((byte)ShapeshifterCooldown);
            writer.Write((byte)ShapeshifterDuration);
            writer.Write((byte)ScientistCooldown);
            writer.Write((byte)GuardianAngelCooldown);
            writer.Write((byte)EngineerCooldown);
            writer.Write((byte)EngineerInVentMaxTime);
            writer.Write((byte)ScientistBatteryCharge);
            writer.Write((byte)ProtectionDurationSeconds);
            writer.Write(ImpostorsCanSeeProtect);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(RoleRates.Count);
            foreach (var roleRate in RoleRates)
            {
                writer.Write((ushort)roleRate.Key);
                writer.Write((byte)roleRate.Value.MaxCount);
                writer.Write((byte)roleRate.Value.Chance);
            }

            writer.Write(ShapeshifterLeaveSkin);
            writer.Write((byte)ShapeshifterCooldown);
            writer.Write((byte)ShapeshifterDuration);
            writer.Write((byte)ScientistCooldown);
            writer.Write((byte)GuardianAngelCooldown);
            writer.Write((byte)EngineerCooldown);
            writer.Write((byte)EngineerInVentMaxTime);
            writer.Write((byte)ScientistBatteryCharge);
            writer.Write((byte)ProtectionDurationSeconds);
            writer.Write(ImpostorsCanSeeProtect);
        }

        public readonly struct RoleRate
        {
            public readonly int MaxCount;
            public readonly int Chance;

            public RoleRate(int maxCount, int chance)
            {
                MaxCount = maxCount;
                Chance = chance;
            }
        }
    }
}
