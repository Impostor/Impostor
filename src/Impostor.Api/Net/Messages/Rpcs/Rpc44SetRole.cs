using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc44SetRole
    {
        public static void Serialize(IMessageWriter writer, RoleTypes role)
        {
            writer.Write((ushort)role);
        }

        public static void Deserialize(IMessageReader reader, out RoleTypes role)
        {
            role = (RoleTypes)reader.ReadUInt16();
        }
    }
}
