using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc12MurderPlayer
    {
        public static void Serialize(IMessageWriter writer, IInnerPlayerControl target, MurderResultFlags result)
        {
            writer.Write(target);
            writer.Write((int)result);
        }

        public static void Deserialize(IMessageReader reader, IGame game, out IInnerPlayerControl? target, out MurderResultFlags result)
        {
            target = reader.ReadNetObject<IInnerPlayerControl>(game);
            result = (MurderResultFlags)reader.ReadInt32();
        }
    }
}
