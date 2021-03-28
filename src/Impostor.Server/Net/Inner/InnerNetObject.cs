using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner
{
    internal abstract partial class InnerNetObject : GameObject, IInnerNetObject
    {
        private const int HostInheritId = -2;

        public uint NetId { get; internal set; }

        public int OwnerId { get; internal set; }

        public SpawnFlags SpawnFlags { get; internal set; }

        public bool IsOwnedBy(IClientPlayer player)
        {
            return OwnerId == player.Client.Id ||
                   (OwnerId == HostInheritId && player.IsHost);
        }

        public abstract ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState);

        public abstract ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState);

        public abstract ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader);

        // TODO move to Reactor.Impostor plugin
        protected ValueTask<bool> HandleCustomRpc(IMessageReader reader, Game game)
        {
            var lengthOrShortId = reader.ReadPackedInt32();

            var pluginId = lengthOrShortId < 0
                ? game.Host!.Client.ModIdMap[lengthOrShortId]
                : reader.ReadString(lengthOrShortId);

            var id = reader.ReadPackedInt32();

            return ValueTask.FromResult(true);
        }
    }
}
