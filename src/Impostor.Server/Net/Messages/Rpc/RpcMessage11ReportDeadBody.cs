using Hazel;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage11ReportDeadBody
    {

        public static void Deserialize(MessageReader reader, out int playerId)
        {
            playerId = reader.ReadPackedInt32();
        }
    }
}