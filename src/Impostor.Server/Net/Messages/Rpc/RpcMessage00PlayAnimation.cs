using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage00PlayAnimation
    {

        public static void Deserialize(MessageReader reader, out byte animationType)
        {
            animationType = reader.ReadByte();
        }
    }
}