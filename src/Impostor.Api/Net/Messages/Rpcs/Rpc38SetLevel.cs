namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc38SetLevel
    {
        public static void Serialize(IMessageWriter writer, uint level)
        {
            writer.WritePacked(level);
        }

        public static void Deserialize(IMessageReader reader, out uint level)
        {
            level = reader.ReadPackedUInt32();
        }
    }
}
