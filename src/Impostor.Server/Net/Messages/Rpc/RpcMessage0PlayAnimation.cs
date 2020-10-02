using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage0PlayAnimation
    {

        public static void Deserialize(MessageReader reader, out byte animationType)
        {
            animationType = reader.ReadByte();
        }
    }
}