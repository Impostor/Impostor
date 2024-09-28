namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc39SetHatStr
    {
        public static void Serialize(IMessageWriter writer, string hat, byte nextRpcSequenceId)
        {
            writer.Write(hat);
            writer.Write(nextRpcSequenceId);
        }

        public static void Deserialize(IMessageReader reader, out string hat, out byte nextRpcSequenceId)
        {
            hat = reader.ReadString();
            nextRpcSequenceId = reader.ReadByte();
        }
    }
}
