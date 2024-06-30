using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc44SetRole
    {
        public static void Serialize(IMessageWriter writer, RoleTypes role, bool canOverrideRole)
        {
            writer.Write((ushort)role);
            writer.Write(canOverrideRole);
        }

        public static void Deserialize(IMessageReader reader, out RoleTypes role, out bool canOverrideRole)
        {
            role = (RoleTypes)reader.ReadUInt16();
            canOverrideRole = reader.ReadBoolean();
        }
    }
}
