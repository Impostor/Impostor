using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Messages
{
    internal static class Message10AlterGame
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.HostGame);
            writer.Write(gameCode);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader, out AlterGameTags gameTag, out bool value)
        {
            gameTag = (AlterGameTags) reader.ReadByte();
            value = reader.ReadBoolean();
        }
    }
}