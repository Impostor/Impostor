using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc12MurderPlayer
    {
        public static void Serialize(IMessageWriter writer, IInnerPlayerControl target)
        {
            writer.Write(target);
        }

        public static void Deserialize(IMessageReader reader, IGame game, out IInnerPlayerControl? target)
        {
            target = reader.ReadNetObject<IInnerPlayerControl>(game);
        }
    }
}
