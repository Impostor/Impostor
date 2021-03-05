namespace Impostor.Api.Net.Messages.C2S
{
    public static class Message01JoinGameC2S
    {
        public static void Serialize(IMessageWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public static void Deserialize(IMessageReader reader, out int gameCode)
        {
            var slice = reader.ReadBytes(sizeof(int)).Span;

            gameCode = slice.ReadInt32();
        }
    }
}
