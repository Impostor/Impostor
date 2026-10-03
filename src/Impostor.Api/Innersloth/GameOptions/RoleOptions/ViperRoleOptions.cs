namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class ViperRoleOptions : IRoleOptions
{
    public ViperRoleOptions(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public RoleTypes Type => RoleTypes.Viper;

    public byte ViperDissolveTime { get; set; } = 15;

    public static ViperRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new ViperRoleOptions(version);

        options.ViperDissolveTime = reader.ReadByte();

        return options;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write((byte)ViperDissolveTime);
    }
}
