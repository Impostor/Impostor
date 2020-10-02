using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage16SendChatNote
    {

        public static void Deserialize(MessageReader reader, out byte playerId, out byte chatNoteType)
        {
            playerId = reader.ReadByte();
            chatNoteType = reader.ReadByte();
        }
    }
}