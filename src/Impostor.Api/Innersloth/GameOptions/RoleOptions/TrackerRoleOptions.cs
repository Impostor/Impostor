namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class TrackerRoleOptions : IRoleOptions
{
    public TrackerRoleOptions(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public RoleTypes Type => RoleTypes.Tracker;

    public byte Cooldown { get; set; } = 15;

    public byte Duration { get; set; } = 30;

    public byte Delay { get; set; } = 1;

    public static TrackerRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new TrackerRoleOptions(version);

        options.Cooldown = reader.ReadByte();
        options.Duration = reader.ReadByte();
        options.Delay = reader.ReadByte();

        return options;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(Cooldown);
        writer.Write(Duration);
        writer.Write(Delay);
    }
}
