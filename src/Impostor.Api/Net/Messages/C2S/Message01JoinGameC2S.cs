using System;
using Impostor.Api.Games;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class Message01JoinGameC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out GameCode gameCode)
        {
            gameCode = reader.ReadInt32();
            reader.ReadBoolean(); // no crossplay
        }
    }
}
