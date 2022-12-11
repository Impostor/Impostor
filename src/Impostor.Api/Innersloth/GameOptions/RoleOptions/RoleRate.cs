namespace Impostor.Api.Innersloth.GameOptions.RoleOptions;

public readonly record struct RoleRate(byte MaxCount, byte Chance)
{
    public static RoleRate Deserialize(IMessageReader reader)
    {
        return new RoleRate(reader.ReadByte(), reader.ReadByte());
    }

    public void Serialize(IMessageWriter writer)
    {
        writer.Write(MaxCount);
        writer.Write(Chance);
    }
}
