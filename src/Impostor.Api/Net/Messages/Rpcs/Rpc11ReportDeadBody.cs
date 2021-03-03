namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc11ReportDeadBody
    {
        public static void Serialize(IMessageWriter writer, byte targetId)
        {
            writer.Write(targetId);
        }

        public static void Deserialize(IMessageReader reader, out byte targetId)
        {
            targetId = reader.ReadByte();
        }
    }
}
