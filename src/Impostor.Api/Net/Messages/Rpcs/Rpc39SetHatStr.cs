namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc39SetHatStr
    {
        public static void Serialize(IMessageWriter writer, string hat)
        {
            writer.Write(hat);
        }

        public static void Deserialize(IMessageReader reader, out string hat)
        {
            hat = reader.ReadString();
        }
    }
}
