using System;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message11KickPlayerS2C
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

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
