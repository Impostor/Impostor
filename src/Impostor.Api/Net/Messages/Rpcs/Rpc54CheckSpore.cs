namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc54CheckSpore
    {
        public static void Serialize(IMessageWriter writer, int mushroomId)
        {
            writer.WritePacked(mushroomId);
        }

        public static void Deserialize(IMessageReader reader, out int mushroomId)
        {
            mushroomId = reader.ReadPackedInt32();
        }
    }
}
