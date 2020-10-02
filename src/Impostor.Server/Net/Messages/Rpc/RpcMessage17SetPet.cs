using Hazel;

namespace Impostor.Server.Net.Messages.Rpc
{
    internal static class RpcMessage17SetPet
    {

        public static void Deserialize(MessageReader reader, out int petId)
        {
            petId = reader.ReadPackedInt32();
        }
    }
}