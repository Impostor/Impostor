using System.Net;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Hazel;

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

        // TODO This no longer does anything, it was replaced by LogicOptions
        public async ValueTask SyncSettingsAsync()
        {
            if (Host?.Character == null)
            {
                throw new ImpostorException("Attempted to set infected when the host was not spawned.");
            }

            using (var writer = StartRpc(Host.Character.NetId, RpcCalls.SyncSettings))
            {
                // Someone will probably forget to do this, so we include it here.
                // If this is not done, the host will overwrite changes later with the defaults.
                Options.IsDefaults = false;

                Rpc02SyncSettings.Serialize(writer, Options);

                await FinishRpcAsync(writer);
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
