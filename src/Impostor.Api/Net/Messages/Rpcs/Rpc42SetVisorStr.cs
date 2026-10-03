namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc42SetVisorStr
    {
        public static void Serialize(IMessageWriter writer, string visor, byte nextRpcSequenceId)
        {
            writer.Write(visor);
            writer.Write(nextRpcSequenceId);
        }

        public static void Deserialize(IMessageReader reader, out string visor, out byte nextRpcSequenceId)
        {
            visor = reader.ReadString();
            nextRpcSequenceId = reader.ReadByte();
        }
    }
}
