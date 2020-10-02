using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage18SetStartCounter
    {

        public static void Deserialize(MessageReader reader, out int secondsLeft)
        {
            secondsLeft = reader.ReadPackedInt32();
        }
    }
}