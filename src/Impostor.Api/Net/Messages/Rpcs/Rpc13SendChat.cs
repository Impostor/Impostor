namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc13SendChat
    {
        public static void Serialize(IMessageWriter writer, string message)
        {
            writer.Write(message);
        }

        public static void Deserialize(IMessageReader reader, out string message)
        {
            message = reader.ReadString();
        }
    }
}
