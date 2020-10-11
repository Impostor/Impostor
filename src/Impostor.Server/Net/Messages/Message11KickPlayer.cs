namespace Impostor.Server.Net.Messages
{
    internal static class Message11KickPlayer
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode, int playerId, bool isBan)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.KickPlayer);
            writer.Write(gameCode);
            writer.WritePacked(playerId);
            writer.Write(isBan);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader, out int playerId, out bool isBan)
        {
            playerId = reader.ReadPackedInt32();
            isBan = reader.ReadBoolean();
        }
    }
}