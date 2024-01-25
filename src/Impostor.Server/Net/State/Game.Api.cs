using System.Net;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;
using Impostor.Server.Events;
using Impostor.Server.Net.Inner;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.State
{
    internal partial class Game : IGame
    {
        private bool _alreadyCallingOptionsChangedEvent = false;

        IClientPlayer? IGame.Host => Host;

        IGameNet IGame.GameNet => GameNet;

        public void BanIp(IPAddress ipAddress)
        {
            _bannedIps.Add(ipAddress);
        }

        public async ValueTask SyncSettingsAsync()
        {
            if (Host?.Character == null)
            {
                throw new ImpostorException("Attempted to change settings when the host was not spawned.");
            }

            if (GameNet.GameManager == null)
            {
                throw new ImpostorException("Attempted to change options when the game manager was not spawned.");
            }

            var gameOptionsTag = GameNet.GameManager.GetGameLogicTag(GameNet.GameManager.LogicOptions);
            if (gameOptionsTag == -1)
            {
                throw new ImpostorException("Attempted to change options when the LogicOptions was not spawned.");
            }

            // Someone will probably forget to do this, so we include it here.
            // If this is not done, the host will overwrite changes later with the defaults.
            Options.IsDefaults = false;

            using var writer = MessageWriter.Get(MessageType.Reliable);

            writer.StartMessage(MessageFlags.GameData);
            Code.Serialize(writer);

            writer.StartMessage(GameDataTag.DataFlag);
            writer.WritePacked(GameNet.GameManager.NetId);

            writer.StartMessage((byte)gameOptionsTag);
            await GameNet.GameManager.LogicOptions.SerializeAsync(writer, false);

            writer.EndMessage();
            writer.EndMessage();
            writer.EndMessage();

            await SendToAllAsync(writer);

            // Prevent bad plugins from causing a server crash by recursing into this function
            if (_alreadyCallingOptionsChangedEvent)
            {
                _logger.LogError("Plugin called SyncSettingsAsync while processing a GameOptionsChangedEvent, aborting to prevent recursion");
            }
            else
            {
                try
                {
                    _alreadyCallingOptionsChangedEvent = true;
                    await _eventManager.CallAsync(new GameOptionsChangedEvent(
                        this,
                        Api.Events.IGameOptionsChangedEvent.ChangeReason.Api
                    ));
                }
                finally
                {
                    _alreadyCallingOptionsChangedEvent = false;
                }
            }
        }

        public async ValueTask SetPrivacyAsync(bool isPublic)
        {
            IsPublic = isPublic;

            using (var writer = MessageWriter.Get(MessageType.Reliable))
            {
                WriteAlterGameMessage(writer, false, IsPublic);

                await SendToAllAsync(writer);
            }
        }
    }
}
