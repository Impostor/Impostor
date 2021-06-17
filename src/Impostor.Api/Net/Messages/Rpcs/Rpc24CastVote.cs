namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc24CastVote
    {
        public static void Serialize(IMessageWriter writer, byte playerId, sbyte suspectPlayerId)
        {
            writer.Write(playerId);
            writer.Write(suspectPlayerId);
        }

        public static void Deserialize(IMessageReader reader, out byte playerId, out byte suspectPlayerId)
        {
            playerId = reader.ReadByte();
            suspectPlayerId = reader.ReadByte();
        }
    }
}
