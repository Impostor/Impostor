namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class EngineerRoleOptions(byte version) : IRoleOptions
{
    public byte Version { get; } = version;

    public byte Cooldown { get; set; } = 30;

    public byte InVentMaxTime { get; set; } = 15;

    public RoleTypes Type
    {
        get => RoleTypes.Engineer;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(Cooldown);
        writer.Write(InVentMaxTime);
    }

    public static EngineerRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new EngineerRoleOptions(version);

        options.Cooldown = reader.ReadByte();
        options.InVentMaxTime = reader.ReadByte();

        return options;
    }
}
