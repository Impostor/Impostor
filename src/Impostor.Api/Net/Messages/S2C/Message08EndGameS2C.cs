using System;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.S2C
{
    public static class Message08EndGameS2C
    {
        public static void Serialize(IMessageWriter writer, GameCode gameCode, GameOverReason gameOverReason)
        {
            writer.StartMessage(MessageFlags.EndGame);
            gameCode.Serialize(writer);
            writer.Write((byte)gameOverReason);

            // Show ad should always be false on custom server
            writer.Write(false);
            writer.EndMessage();
        }

        public static void Deserialize()
        {
            throw new NotImplementedException();
        }
    }
}
