using Impostor.Api.Innersloth.Customization;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc07CheckColor
    {
        public static void Serialize(IMessageWriter writer, ColorType color)
        {
            writer.Write((byte)color);
        }

        public static void Deserialize(IMessageReader reader, out ColorType color)
        {
            color = (ColorType)reader.ReadByte();
        }
    }
}
