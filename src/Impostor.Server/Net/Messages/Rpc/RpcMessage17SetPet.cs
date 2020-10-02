using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage17SetPet
    {

        public static void Deserialize(MessageReader reader, out byte petId)
        {
            petId = reader.ReadByte();
        }
    }
}