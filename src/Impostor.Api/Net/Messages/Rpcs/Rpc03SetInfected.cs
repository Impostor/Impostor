using System;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc03SetInfected
    {
        public static void Serialize(IMessageWriter writer, byte[] infectedIds)
        {
            writer.WriteBytesAndSize(infectedIds);
        }

        public static void Deserialize(IMessageReader reader, out ReadOnlyMemory<byte> infectedIds)
        {
            infectedIds = reader.ReadBytesAndSize();
        }
    }
}
