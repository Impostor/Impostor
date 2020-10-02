using Hazel;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage1CompleteTask
    {

        public static void Deserialize(MessageReader reader, out int taskId)
        {
            taskId = reader.ReadPackedInt32();
        }
    }
}