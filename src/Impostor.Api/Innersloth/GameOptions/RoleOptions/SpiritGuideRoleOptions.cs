namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class SpiritGuideRoleOptions : IRoleOptions
{
    public SpiritGuideRoleOptions(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public RoleTypes Type => RoleTypes.SpiritGuide;

    public float SpiritGuideCooldownSeconds { get; set; } = 20f;

    public static SpiritGuideRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new SpiritGuideRoleOptions(version);

        options.SpiritGuideCooldownSeconds = reader.ReadByte();

        return options;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write((byte)SpiritGuideCooldownSeconds);
    }
}
