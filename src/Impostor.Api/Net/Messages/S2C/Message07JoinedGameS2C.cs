using System;

namespace Impostor.Api.Net.Messages.S2C
{
    public static class Message07JoinedGameS2C
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode, int playerId, int hostId, int[] otherPlayerIds)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
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

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
