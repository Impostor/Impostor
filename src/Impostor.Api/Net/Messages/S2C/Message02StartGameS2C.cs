using System;
using Impostor.Api.Games;

namespace Impostor.Api.Net.Messages.S2C
{
    public static class Message02StartGameS2C
    {
        public static void Serialize(IMessageWriter writer, GameCode gameCode)
        {
            writer.StartMessage(MessageFlags.StartGame);
            gameCode.Serialize(writer);
            writer.EndMessage();
        }

        public static void Deserialize()
        {
            throw new NotImplementedException();
        }
    }
}
