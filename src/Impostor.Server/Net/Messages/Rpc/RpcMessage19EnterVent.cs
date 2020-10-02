using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage19EnterVent
    {

        public static void Deserialize(MessageReader reader, out int ventId)
        {
            ventId = reader.ReadPackedInt32();
        }
    }
}