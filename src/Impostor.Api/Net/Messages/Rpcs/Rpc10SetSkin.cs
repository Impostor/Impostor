using Impostor.Api.Innersloth.Customization;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc10SetSkin
    {
        public static void Serialize(IMessageWriter writer, SkinType skin)
        {
            writer.WritePacked((uint)skin);
        }

        public static void Deserialize(IMessageReader reader, out SkinType skin)
        {
            skin = (SkinType)reader.ReadPackedUInt32();
        }
    }
}
