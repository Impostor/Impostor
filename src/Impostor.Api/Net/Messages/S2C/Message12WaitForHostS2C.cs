using System;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message12WaitForHostS2C
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode, int playerId)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.WaitForHost);
            writer.Write(gameCode);
            writer.Write(playerId);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
