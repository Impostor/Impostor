using System.Threading.Tasks;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth;
using Impostor.Shared.Innersloth.Data;

namespace Impostor.Server.Net.State
{
    internal partial class ClientPlayer
    {
        /// <summary>
        ///     Triggered when the connected client requests the game listing.
        /// </summary>
        /// <param name="options">
        ///     All options given.
        ///     At this moment, the client can only specify the map, impostor count and chat language.
        /// </param>
        public async ValueTask OnRequestGameList(GameOptionsData options)
        {
            using (var message = Client.Connection.CreateMessage(MessageType.Reliable))
            {
                var games = _gameManager.FindListings((MapFlags) options.MapId, options.NumImpostors, options.Keywords);

                var skeldGameCount = _gameManager.GetGameCount(MapFlags.Skeld);
                var miraHqGameCount = _gameManager.GetGameCount(MapFlags.MiraHQ);
                var polusGameCount = _gameManager.GetGameCount(MapFlags.Polus);

                Message16GetGameListV2.Serialize(message, skeldGameCount, miraHqGameCount, polusGameCount, games);

                await message.SendAsync();
            }
        }
    }
}