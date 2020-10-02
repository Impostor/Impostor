using Hazel;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage13SendChat
    {

        public static void Deserialize(MessageReader reader, out int rpcChatLength, out byte[] rpcChatMessage)
        {
            rpcChatLength = reader.ReadPackedInt32();
            rpcChatMessage = reader.ReadBytes(rpcChatLength);
        }
    }
}