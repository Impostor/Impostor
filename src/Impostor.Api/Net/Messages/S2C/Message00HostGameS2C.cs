using Impostor.Api.Games;

namespace Impostor.Api.Net.Messages.S2C
{
    public static class Message00HostGameS2C
    {
        public static void Serialize(IMessageWriter writer, GameCode gameCode)
        {
            writer.StartMessage(MessageFlags.HostGame);
            gameCode.Serialize(writer);
            writer.EndMessage();
        }

        public static void Deserialize(IMessageReader reader, out GameCode gameCode)
        {
            gameCode = reader.ReadInt32();
        }
    }
}
