namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc41SetPetStr
    {
        public static void Serialize(IMessageWriter writer, string pet, byte nextRpcSequenceId)
        {
            writer.Write(pet);
            writer.Write(nextRpcSequenceId);
        }

        public static void Deserialize(IMessageReader reader, out string pet, out byte nextRpcSequenceId)
        {
            pet = reader.ReadString();
            nextRpcSequenceId = reader.ReadByte();
        }
    }
}
