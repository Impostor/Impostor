using Impostor.Api.Innersloth.Customization;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc09SetHat
    {
        public static void Serialize(IMessageWriter writer, HatType hat)
        {
            writer.WritePacked((uint)hat);
        }

        public static void Deserialize(IMessageReader reader, out HatType hat)
        {
            hat = (HatType)reader.ReadPackedUInt32();
        }
    }
}
