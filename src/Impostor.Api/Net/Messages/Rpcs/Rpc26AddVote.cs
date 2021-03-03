namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc26AddVote
    {
        public static void Serialize(IMessageWriter writer, int clientId, int targetClientId)
        {
            writer.Write(clientId);
            writer.Write(targetClientId);
        }

        public static void Deserialize(IMessageReader reader, out int clientId, out int targetClientId)
        {
            clientId = reader.ReadInt32();
            targetClientId = reader.ReadInt32();
        }
    }
}
