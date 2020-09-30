using Hazel;

namespace Impostor.Server.Net.Messages
{
    internal static class Message07JoinedGame
    {
        public static void Serialize(MessageWriter writer, bool clear, int gameCode, int playerId, int hostId, int[] otherPlayerIds)
        {
            if (clear)
            {
                writer.Clear(SendOption.Reliable);
            }
            
            writer.StartMessage(MessageFlags.JoinedGame);
            writer.Write(gameCode);
            writer.Write(playerId);
            writer.Write(hostId);
            writer.WritePacked(otherPlayerIds.Length);

            foreach (var id in otherPlayerIds)
            {
                writer.WritePacked(id);
            }
            
            writer.EndMessage();
        }
    }
}