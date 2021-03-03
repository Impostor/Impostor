namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc06SetName
    {
        public static void Serialize(IMessageWriter writer, string name)
        {
            writer.Write(name);
        }

        public static void Deserialize(IMessageReader reader, out string name)
        {
            name = reader.ReadString();
        }
    }
}
