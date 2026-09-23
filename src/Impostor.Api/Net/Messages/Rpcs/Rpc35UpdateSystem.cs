using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc35UpdateSystem
    {
        public static void Serialize(IMessageWriter writer, SystemTypes systemType, IInnerPlayerControl playerControl, IMessageWriter payload)
        {
            writer.Write((byte)systemType);
            writer.Write(playerControl);
            writer.Write(payload, false);
        }

        public static void Serialize(IMessageWriter writer, SystemTypes systemType, IInnerPlayerControl playerControl, byte amount)
        {
            writer.Write((byte)systemType);
            writer.Write(playerControl);
            writer.Write(amount);
        }

        public static void Deserialize(IMessageReader reader, IGame game, out SystemTypes systemType, out IInnerPlayerControl? playerControl, out IMessageReader payload)
        {
            systemType = (SystemTypes)reader.ReadByte();
            playerControl = reader.ReadNetObject<IInnerPlayerControl>(game);

            // The system payload is appended directly with Write(payload, false); it is not a nested message.
            payload = reader;
        }
    }
}
