using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage9SetHat
    {

        public static void Deserialize(MessageReader reader, out int hatId)
        {
            hatId = reader.ReadPackedInt32();
        }
    }
}