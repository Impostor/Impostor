namespace Impostor.Api.Net.Messages.Auth
{
    public static class Message01Complete
    {
        public static void Serialize(IMessageWriter writer, uint nonce)
        {
            writer.WritePacked(nonce);
        }

        public static void Deserialize(IMessageReader reader, out uint nonce)
        {
            nonce = reader.ReadUInt32();
        }
    }
}
