using System;

namespace Impostor.Api.Net.Messages.C2S
{
    public static class Message01JoinGameC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out int gameCode, out byte unknown)
        {
            var slice = reader.ReadBytes(sizeof(Int32) + sizeof(byte)).Span;

            gameCode = slice.ReadInt32();
            unknown = slice.ReadByte();
        }
    }
}