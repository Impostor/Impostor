using System;

namespace Impostor.Api.Net.Messages.C2S
{
    public class Message04RemovePlayerC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out int playerId, out byte reason)
        {
            playerId = reader.ReadPackedInt32();
            reason = reader.ReadByte();
        }
    }
}
