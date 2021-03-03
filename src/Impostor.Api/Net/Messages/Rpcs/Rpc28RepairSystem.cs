using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc28RepairSystem
    {
        public static void Serialize(IMessageWriter writer, SystemTypes systemType, IInnerPlayerControl player, byte amount)
        {
            writer.Write((byte)systemType);
            writer.Write(player);
            writer.Write(amount);
        }

        public static void Deserialize(IMessageReader reader, IGame game, out SystemTypes systemType, out IInnerPlayerControl? player, out byte amount)
        {
            systemType = (SystemTypes)reader.ReadByte();
            player = reader.ReadNetObject<IInnerPlayerControl>(game);
            amount = reader.ReadByte();
        }
    }
}
