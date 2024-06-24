namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc64CheckAppear
    {
        public static void Serialize(IMessageWriter writer, bool shouldAnimate)
        {
            writer.Write(shouldAnimate);
        }

        public static void Deserialize(IMessageReader reader, out bool shouldAnimate)
        {
            shouldAnimate = reader.ReadBoolean();
        }
    }
}
