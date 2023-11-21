namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc52UseZipline
    {
        public static void Serialize(IMessageWriter writer, bool fromTop)
        {
            writer.Write(fromTop);
        }

        public static void Deserialize(IMessageReader reader, out bool fromTop)
        {
            fromTop = reader.ReadBoolean();
        }
    }
}
