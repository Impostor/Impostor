namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc01CompleteTask
    {
        public static void Serialize(IMessageWriter writer, uint taskId)
        {
            writer.WritePacked(taskId);
        }

        public static void Deserialize(IMessageReader reader, out uint taskId)
        {
            taskId = reader.ReadPackedUInt32();
        }
    }
}
