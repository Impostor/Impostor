using System;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc29SetTasks
    {
        public static void Serialize(IMessageWriter writer, ReadOnlyMemory<byte> taskTypeIds)
        {
            writer.Write(taskTypeIds);
        }

        public static void Deserialize(IMessageReader reader, out ReadOnlyMemory<byte> taskTypeIds)
        {
            taskTypeIds = reader.ReadBytesAndSize();
        }
    }
}
