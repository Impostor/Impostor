namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class ScientistRoleOptions : IRoleOptions
{
    public ScientistRoleOptions(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public RoleTypes Type => RoleTypes.Scientist;

    public byte Cooldown { get; set; } = 15;

    public byte BatteryCharge { get; set; } = 5;

    public static ScientistRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new ScientistRoleOptions(version);

        options.Cooldown = reader.ReadByte();
        options.BatteryCharge = reader.ReadByte();

        return options;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(Cooldown);
        writer.Write(BatteryCharge);
    }
}
