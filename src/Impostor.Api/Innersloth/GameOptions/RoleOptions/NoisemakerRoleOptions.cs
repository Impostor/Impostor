namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class NoisemakerRoleOptions : IRoleOptions
{
    public NoisemakerRoleOptions(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public bool ImpostorAlert { get; set; } = true;

    public byte AlertDuration { get; set; } = 10;

    public RoleTypes Type
    {
        get => RoleTypes.Noisemaker;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(AlertDuration);
        writer.Write(ImpostorAlert);
    }

    public static NoisemakerRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new NoisemakerRoleOptions(version);

        options.AlertDuration = reader.ReadByte();
        options.ImpostorAlert = reader.ReadBoolean();

        return options;
    }
}
