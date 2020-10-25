using System.IO;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Server.Net.Inner;

namespace Impostor.Server.Net.State
{
    internal partial class Game : IGame
    {
        IClientPlayer IGame.Host => Host;

        IGameNet IGame.GameNet => GameNet;

        public async ValueTask SyncSettingsAsync()
        {
            using (var writer = StartRpc(Host.Character.NetId, RpcCalls.SyncSettings))
            {
                // Someone will probably forget to do this, so we include it here.
                // If this is not done, the host will overwrite changes later with the defaults.
                Options.IsDefaults = false;

                await using (var memory = new MemoryStream())
                await using (var writerBin = new BinaryWriter(memory))
                {
                    Options.Serialize(writerBin, GameOptionsData.LatestVersion);
                    writer.WriteBytesAndSize(memory.ToArray());
                }

                await FinishRpcAsync(writer);
            }
        }
    }
}