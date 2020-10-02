using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage16SendChatNote
    {

        public static void Deserialize(MessageReader reader, out int playerId, out int chatNoteType)
        {
            playerId = reader.ReadPackedInt32();
            chatNoteType = reader.ReadPackedInt32();
        }
    }
}