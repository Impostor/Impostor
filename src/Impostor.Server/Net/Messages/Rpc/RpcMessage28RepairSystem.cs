using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage28RepairSystem
    {

        public static void Deserialize(MessageReader reader, out int systemType, out int amount)
        {
            systemType = reader.ReadPackedInt32();
            amount = reader.ReadPackedInt32();
        }
    }
}