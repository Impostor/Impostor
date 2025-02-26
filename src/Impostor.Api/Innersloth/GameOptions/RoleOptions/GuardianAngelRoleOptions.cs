namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class GuardianAngelRoleOptions(byte version) : IRoleOptions
{
    public byte Version { get; } = version;

    public byte Cooldown { get; set; } = 60;

    public byte ProtectionDurationSeconds { get; set; } = 10;

    public bool ImpostorsCanSeeProtect { get; set; }

    public RoleTypes Type
    {
        get => RoleTypes.GuardianAngel;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(Cooldown);
        writer.Write(ProtectionDurationSeconds);
        writer.Write(ImpostorsCanSeeProtect);
    }

    public static GuardianAngelRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new GuardianAngelRoleOptions(version);

        options.Cooldown = reader.ReadByte();
        options.ProtectionDurationSeconds = reader.ReadByte();
        options.ImpostorsCanSeeProtect = reader.ReadBoolean();

        return options;
    }
}
