using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage10SetSkin
    {

        public static void Deserialize(MessageReader reader, out byte skinId)
        {
            skinId = reader.ReadByte();
        }
    }
}