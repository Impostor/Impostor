using System;

namespace Impostor.Api.Net.Messages.Rpcs;

public static class Rpc67SpiritGuideMessage
{
    public static void Serialize(IMessageWriter writer, byte[] spriteIndices)
    {
        writer.WriteBytesAndSize(spriteIndices);
    }

    public static void Deserialize(IMessageReader reader, out ReadOnlyMemory<byte> spriteIndices)
    {
        spriteIndices = reader.ReadBytesAndSize();
    }
}
