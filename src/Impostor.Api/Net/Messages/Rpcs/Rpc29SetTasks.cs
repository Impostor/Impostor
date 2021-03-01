using System;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc29SetTasks
    {
        public static void Serialize(IMessageWriter writer, byte playerId, ReadOnlyMemory<byte> taskTypeIds)
        {
            writer.Write(playerId);
            writer.Write(taskTypeIds);
        }

        public static void Deserialize(IMessageReader reader, out byte playerId, out ReadOnlyMemory<byte> taskTypeIds)
        {
            playerId = reader.ReadByte();
            taskTypeIds = reader.ReadBytesAndSize();
        }
    }
}
