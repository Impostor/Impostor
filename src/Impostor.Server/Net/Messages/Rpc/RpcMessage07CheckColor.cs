using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage07CheckColor
    {

        public static void Deserialize(MessageReader reader, out byte colorId)
        {
            colorId = reader.ReadByte();
        }
    }
}