using Impostor.Api.Games;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Net.Messages.Rpcs
{
    public static class Rpc45ProtectPlayer
    {
        public static void Serialize(IMessageWriter writer, IInnerPlayerControl playerControl, ColorType color)
        {
            writer.Write(playerControl);
            writer.Write((byte)color);
        }

        public static void Deserialize(IMessageReader reader, IGame game, out IInnerPlayerControl? playerControl, out ColorType color)
        {
            playerControl = reader.ReadNetObject<IInnerPlayerControl>(game);
            color = (ColorType)reader.ReadByte();
        }
    }
}
