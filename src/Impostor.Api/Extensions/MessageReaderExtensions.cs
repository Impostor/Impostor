using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Api
{
    public static class MessageReaderExtensions
    {
        public static T ReadNetObject<T>(this IMessageReader reader, IGame game)
            where T : InnerNetObject
        {
            return game.FindObjectByNetId<T>(reader.ReadPackedUInt32());
        }
    }
}