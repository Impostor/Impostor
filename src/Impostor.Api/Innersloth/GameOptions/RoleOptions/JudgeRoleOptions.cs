namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public class JudgeRoleOptions : IRoleOptions
{
    public JudgeRoleOptions(byte version)
    {
        Version = version;
    }

    public byte Version { get; }

    public RoleTypes Type => RoleTypes.Judge;

    public float JudgeTaskRequirementPercentage { get; set; } = 50f;

    public static JudgeRoleOptions Deserialize(IMessageReader reader, byte version)
    {
        var options = new JudgeRoleOptions(version);

        options.JudgeTaskRequirementPercentage = (int)reader.ReadByte();

        return options;
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write((byte)JudgeTaskRequirementPercentage);
    }
}
