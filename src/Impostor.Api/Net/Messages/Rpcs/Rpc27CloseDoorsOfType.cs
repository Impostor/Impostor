using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc27CloseDoorsOfType
    {
        public static void Serialize(IMessageWriter writer, SystemTypes systemType)
        {
            writer.Write((byte)systemType);
        }

        public static void Deserialize(IMessageReader reader, out SystemTypes systemType)
        {
            systemType = (SystemTypes)reader.ReadByte();
        }
    }
}
