using System;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message02StartGameS2C
    {
        public static void Serialize(IMessageWriter writer, bool clear, int gameCode)
        {
            if (clear)
            {
                writer.Clear(MessageType.Reliable);
            }

            writer.StartMessage(MessageFlags.StartGame);
            writer.Write(gameCode);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
