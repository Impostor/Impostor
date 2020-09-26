using Hazel;
using Impostor.Server.Net.Messages;
using Impostor.Shared.Innersloth;

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
        public void OnRequestGameList(GameOptionsData options)
        {
            using (var message = MessageWriter.Get(SendOption.Reliable))
            {
                var games = _gameManager.FindListings(options.MapId, options.NumImpostors, options.Keywords);
                
                Message16GetGameListV2.Serialize(message, games);
                
                Client.Send(message);
            }
        }
    }
}