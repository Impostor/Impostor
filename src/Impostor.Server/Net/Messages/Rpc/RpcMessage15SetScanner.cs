using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage15SetScanner
    {

        public static void Deserialize(MessageReader reader, out bool enabled)
        {
            enabled = reader.ReadBoolean();
        }
    }
}