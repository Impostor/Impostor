using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage28RepairSystem
    {

        public static void Deserialize(MessageReader reader, out byte systemType, out byte amount)
        {
            systemType = reader.ReadByte();
            amount = reader.ReadByte();
        }
    }
}