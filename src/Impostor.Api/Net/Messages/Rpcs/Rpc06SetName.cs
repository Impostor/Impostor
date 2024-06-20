namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc06SetName
    {
        public static void Serialize(IMessageWriter writer, uint netId, string name)
        {
            writer.Write(netId);
            writer.Write(name);
        }

        public static void Deserialize(IMessageReader reader, out uint netId, out string name)
        {
            netId = reader.ReadUInt32();
            name = reader.ReadString();
        }
    }
}
