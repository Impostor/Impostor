using Impostor.Api.Net.Messages;
using Impostor.Server.Net.Inner;
using Impostor.Server.Net.State;

namespace Impostor.Server
{
    internal static class MessageReaderExtensions
    {
        public static T ReadNetObject<T>(this IMessageReader reader, Game game)
            where T : InnerNetObject
        {
            return game.FindObjectByNetId<T>(reader.ReadPackedUInt32());
        }
    }
}