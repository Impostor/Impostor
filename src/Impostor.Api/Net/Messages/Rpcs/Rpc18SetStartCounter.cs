namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc18SetStartCounter
    {
        public static void Serialize(IMessageWriter writer, int sequenceId, sbyte startCounter)
        {
            writer.Write(sequenceId);
            writer.Write(startCounter);
        }

        public static void Deserialize(IMessageReader reader, out int sequenceId, out sbyte startCounter)
        {
            sequenceId = reader.ReadPackedInt32();
            startCounter = reader.ReadSByte();
        }
    }
}
