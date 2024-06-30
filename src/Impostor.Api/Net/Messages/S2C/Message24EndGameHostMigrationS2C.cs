using Impostor.Api.Games;

namespace Impostor.Api.Net.Messages.S2C
{
    public class Message24EndGameHostMigrationS2C
    {
        public static void Serialize(IMessageWriter writer, GameCode gameCode, int hostClientId)
        {
            writer.StartMessage(MessageFlags.EndGameHostMigration);
            gameCode.Serialize(writer);
            writer.Write(hostClientId);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader, out GameCode gameCode, out int hostClientId)
        {
            gameCode = reader.ReadInt32();
            hostClientId = reader.ReadInt32();
        }
    }
}
