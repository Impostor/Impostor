using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc48CheckProtect
    {
        public static void Serialize(IMessageWriter writer, IInnerPlayerControl playerControl)
        {
            writer.Write(playerControl);
        }

        public static void Deserialize(IMessageReader reader, IGame game, out IInnerPlayerControl? playerControl)
        {
            playerControl = reader.ReadNetObject<IInnerPlayerControl>(game);
        }
    }
}
