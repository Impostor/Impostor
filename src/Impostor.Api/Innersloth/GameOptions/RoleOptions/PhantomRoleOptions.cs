namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class PhantomRoleOptions(byte version) : IRoleOptions
{
    public byte Version { get; } = version;

    public byte Cooldown { get; set; } = 15;

    public byte Duration { get; set; } = 30;

    public RoleTypes Type
    {
        get => RoleTypes.Phantom;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(Cooldown);
        writer.Write(Duration);
    }

    public static PhantomRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new PhantomRoleOptions(version);

        options.Cooldown = reader.ReadByte();
        options.Duration = reader.ReadByte();

        return options;
    }
}
