using System.Net;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;
using Impostor.Hazel;
using Impostor.Server.Net.Inner;
using Impostor.Server.Net.Inner.Objects.GameManager;

namespace Impostor.Server.Net.State
{
    internal partial class Game : IGame
    {
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

            var gameManager = FindObjectByType<InnerGameManager>();
            if (gameManager == null)
            {
                throw new ImpostorException("Attempted to change options when the game manager was not spawned.");
            }

            var gameOptionsTag = gameManager.GetGameLogicTag(gameManager.LogicOptions);
            if (gameOptionsTag == null)
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
            writer.WritePacked(gameManager.NetId);

            writer.StartMessage((byte)gameOptionsTag);
            GameOptionsFactory.Serialize(writer, Options);

            writer.EndMessage();
            writer.EndMessage();
            writer.EndMessage();

            await SendToAllAsync(writer);
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
