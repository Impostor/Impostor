namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc31ClimbLadder
    {
        public static void Serialize(IMessageWriter writer, byte ladderId, byte lastClimbLadderSid)
        {
            writer.Write(ladderId);
            writer.Write(lastClimbLadderSid);
        }

        public static void Deserialize(IMessageReader reader, out byte ladderId, out byte lastClimbLadderSid)
        {
            ladderId = reader.ReadByte();
            lastClimbLadderSid = reader.ReadByte();
        }
    }
}
