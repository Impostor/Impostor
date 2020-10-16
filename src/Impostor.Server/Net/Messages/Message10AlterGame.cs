using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Messages
{
    internal static class Message10AlterGame
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode, bool isPublic)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.AlterGame);
            writer.Write(gameCode);
            writer.Write((byte)AlterGameTags.ChangePrivacy);
            writer.Write(isPublic);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader, out AlterGameTags gameTag, out bool isPublic)
        {
            gameTag = (AlterGameTags) reader.ReadByte();
            isPublic = reader.ReadBoolean();
        }
    }
}