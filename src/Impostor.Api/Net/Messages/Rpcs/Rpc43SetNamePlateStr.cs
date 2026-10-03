namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc43SetNamePlateStr
    {
        public static void Serialize(IMessageWriter writer, string namePlate, byte nextRpcSequenceId)
        {
            writer.Write(namePlate);
            writer.Write(nextRpcSequenceId);
        }

        public static void Deserialize(IMessageReader reader, out string namePlate, out byte nextRpcSequenceId)
        {
            namePlate = reader.ReadString();
            nextRpcSequenceId = reader.ReadByte();
        }
    }
}
