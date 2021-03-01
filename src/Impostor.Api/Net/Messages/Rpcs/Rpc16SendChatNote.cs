using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc16SendChatNote
    {
        public static void Serialize(IMessageWriter writer, byte playerId, ChatNoteType chatNoteType)
        {
            writer.Write(playerId);
            writer.Write((byte)chatNoteType);
        }

        public static void Deserialize(IMessageReader reader, out byte playerId, out ChatNoteType chatNoteType)
        {
            playerId = reader.ReadByte();
            chatNoteType = (ChatNoteType)reader.ReadByte();
        }
    }
}
