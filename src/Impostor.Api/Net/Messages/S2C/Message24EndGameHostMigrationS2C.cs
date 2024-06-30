using System;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message24EndGameHostMigrationS2C
    {
        public static void Serialize(IMessageWriter writer, int gameCode, int hostClientId)
        {
            writer.StartMessage(MessageFlags.EndGameHostMigration);
            writer.Write(gameCode);
            writer.Write(hostClientId);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
