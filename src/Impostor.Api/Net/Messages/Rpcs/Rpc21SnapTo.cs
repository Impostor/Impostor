using System.Numerics;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc21SnapTo
    {
        public static void Serialize(IMessageWriter writer, Vector2 position, ushort minSid)
        {
            writer.Write(position);
            writer.Write(minSid);
        }

        public static void Deserialize(IMessageReader reader, out Vector2 position, out ushort minSid)
        {
            position = reader.ReadVector2();
            minSid = reader.ReadUInt16();
        }
    }
}
