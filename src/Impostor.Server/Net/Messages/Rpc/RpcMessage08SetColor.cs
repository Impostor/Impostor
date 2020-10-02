using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage08SetColor
    {

        public static void Deserialize(MessageReader reader, out byte colorId)
        {
            colorId = reader.ReadByte();
        }
    }
}