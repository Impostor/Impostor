using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage10SetSkin
    {

        public static void Deserialize(MessageReader reader, out int skinId)
        {
            skinId = reader.ReadPackedInt32();
        }
    }
}