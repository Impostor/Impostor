using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage6SetName
    {

        public static void Deserialize(MessageReader reader, out string name)
        {
            name = reader.ReadString();
        }
    }
}