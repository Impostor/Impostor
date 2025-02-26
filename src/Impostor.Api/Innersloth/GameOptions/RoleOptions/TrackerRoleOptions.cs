namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class TrackerRoleOptions(byte version) : IRoleOptions
{
    public byte Version { get; } = version;

    public byte Cooldown { get; set; } = 15;

    public byte Duration { get; set; } = 30;

    public byte Delay { get; set; } = 1;

    public RoleTypes Type
    {
        get => RoleTypes.Tracker;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(Cooldown);
        writer.Write(Duration);
        writer.Write(Delay);
    }

    public static TrackerRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new TrackerRoleOptions(version);

        options.Cooldown = reader.ReadByte();
        options.Duration = reader.ReadByte();
        options.Delay = reader.ReadByte();

        return options;
    }
}
