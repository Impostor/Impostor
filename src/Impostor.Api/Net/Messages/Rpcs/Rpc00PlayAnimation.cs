using Impostor.Api.Innersloth;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc00PlayAnimation
    {
        public static void Serialize(IMessageWriter writer, TaskTypes task)
        {
            writer.Write((byte)task);
        }

        public static void Deserialize(IMessageReader reader, out TaskTypes task)
        {
            task = (TaskTypes)reader.ReadByte();
        }
    }
}
