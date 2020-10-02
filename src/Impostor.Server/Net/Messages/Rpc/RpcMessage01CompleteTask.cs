using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage01CompleteTask
    {

        public static void Deserialize(MessageReader reader, out byte taskId)
        {
            taskId = reader.ReadByte();
        }
    }
}