namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class DetectiveRoleOptions : IRoleOptions
{
    public DetectiveRoleOptions(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public RoleTypes Type => RoleTypes.Detective;

    public byte DetectiveSuspectLimit { get; set; } = 3;

    public static DetectiveRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new DetectiveRoleOptions(version);

        options.DetectiveSuspectLimit = reader.ReadByte();

        return options;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write((byte)DetectiveSuspectLimit);
    }
}
