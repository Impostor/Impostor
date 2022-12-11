using System.Numerics;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc49Pet
    {
        public static void Serialize(IMessageWriter writer, Vector2 position, Vector2 petPosition)
        {
            writer.Write(position);
            writer.Write(petPosition);
        }

        public static void Deserialize(IMessageReader reader, out Vector2 position, out Vector2 petPosition)
        {
            position = reader.ReadVector2();
            petPosition = reader.ReadVector2();
        }
    }
}
