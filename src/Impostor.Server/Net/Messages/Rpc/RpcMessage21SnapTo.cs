using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage21SnapTo
    {

        public static void Deserialize(MessageReader reader, out float x, out float y)
        {
            // Assumed floats because vector2
            x = reader.ReadSingle();
            y = reader.ReadSingle();
        }
    }
}