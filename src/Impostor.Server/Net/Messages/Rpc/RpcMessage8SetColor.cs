using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage8SetColor
    {

        public static void Deserialize(MessageReader reader, out int colorId)
        {
            colorId = reader.ReadPackedInt32();
        }
    }
}