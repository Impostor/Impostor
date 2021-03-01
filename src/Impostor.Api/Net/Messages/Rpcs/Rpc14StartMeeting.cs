namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc14StartMeeting
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
