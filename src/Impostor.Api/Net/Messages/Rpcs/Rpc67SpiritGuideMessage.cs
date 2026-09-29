using System;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc67SpiritGuideMessage
    {
        public static void Serialize(IMessageWriter writer, byte[] images)
        {
            writer.WriteBytesAndSize(images);
        }

        public static void Deserialize(IMessageReader reader, out ReadOnlyMemory<byte> images)
        {
            images = reader.ReadBytesAndSize();
        }
    }
}
