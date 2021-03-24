using System;

namespace Impostor.Api.Net.Messages.C2S
{
    public class Message11KickPlayerC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out int playerId, out bool isBan)
        {
            playerId = reader.ReadPackedInt32();
            isBan = reader.ReadBoolean();
        }
    }
}
