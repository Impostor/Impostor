namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class ShapeshifterRoleOptions : IRoleOptions
{
    public ShapeshifterRoleOptions(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public bool LeaveSkin { get; set; }

    public byte Cooldown { get; set; } = 10;

    public byte Duration { get; set; } = 30;

    public RoleTypes Type => RoleTypes.Shapeshifter;

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(LeaveSkin);
        writer.Write(Cooldown);
        writer.Write(Duration);
    }

    public static ShapeshifterRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new ShapeshifterRoleOptions(version);

        options.LeaveSkin = reader.ReadBoolean();
        options.Cooldown = reader.ReadByte();
        options.Duration = reader.ReadByte();

        return options;
    }
}
