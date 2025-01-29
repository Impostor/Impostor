using System;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message11KickPlayerS2C
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode, int playerId, bool isBan, DisconnectReason? reason = null)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.KickPlayer);
            writer.Write(gameCode);
            writer.WritePacked(playerId);
            writer.Write(isBan);

            // If no reason is provided, Client will handle it as DisconnectReason.Kick or DisconnectReason.Ban depending on isBan
            if (reason != null)
            {
                writer.Write((byte)reason);
            }

            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
