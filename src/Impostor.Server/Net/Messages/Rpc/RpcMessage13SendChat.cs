using Hazel;
using Impostor.Shared.Innersloth.Data;

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