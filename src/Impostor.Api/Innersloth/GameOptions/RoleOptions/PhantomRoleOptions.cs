namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class PhantomRoleOptions : IRoleOptions
{
    public PhantomRoleOptions(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public RoleTypes Type => RoleTypes.Phantom;

    public byte Cooldown { get; set; } = 15;

    public byte Duration { get; set; } = 30;

    public static PhantomRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new PhantomRoleOptions(version);

        options.Cooldown = reader.ReadByte();
        options.Duration = reader.ReadByte();

        return options;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(Cooldown);
        writer.Write(Duration);
    }
}
