using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage20ExitVent
    {

        public static void Deserialize(MessageReader reader, out int ventId)
        {
            ventId = reader.ReadPackedInt32();
        }
    }
}