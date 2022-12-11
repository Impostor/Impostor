using System;
using Impostor.Api.Innersloth.GameOptions;

namespace Impostor.Api.Net.Messages.S2C
{
    public static class Message00HostGameS2C
    {
        public static void Serialize(IMessageWriter writer, int gameCode)
        {
            writer.StartMessage(MessageFlags.HostGame);
            writer.Write(gameCode);
            writer.EndMessage();
        }

        public static LegacyGameOptionsData Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
