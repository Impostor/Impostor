namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc41SetPetStr
    {
        public static void Serialize(IMessageWriter writer, string pet)
        {
            writer.Write(pet);
        }

        public static void Deserialize(IMessageReader reader, out string pet)
        {
            pet = reader.ReadString();
        }
    }
}
