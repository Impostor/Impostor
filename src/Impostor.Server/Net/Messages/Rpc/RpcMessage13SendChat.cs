using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage13SendChat
    {

        public static void Deserialize(MessageReader reader, out string rpcChatMessage)
        {
            rpcChatMessage = reader.ReadString();
        }
    }
}