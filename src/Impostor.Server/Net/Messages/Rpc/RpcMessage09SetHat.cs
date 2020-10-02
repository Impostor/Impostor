using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage09SetHat
    {

        public static void Deserialize(MessageReader reader, out byte hatId)
        {
            hatId = reader.ReadByte();
        }
    }
}