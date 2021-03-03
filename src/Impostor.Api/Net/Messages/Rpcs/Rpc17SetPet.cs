using Impostor.Api.Innersloth.Customization;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc17SetPet
    {
        public static void Serialize(IMessageWriter writer, PetType pet)
        {
            writer.WritePacked((uint)pet);
        }

        public static void Deserialize(IMessageReader reader, out PetType pet)
        {
            pet = (PetType)reader.ReadPackedUInt32();
        }
    }
}
