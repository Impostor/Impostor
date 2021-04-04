using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner
{
    internal abstract partial class InnerNetObject : GameObject, IInnerNetObject
    {
        private const int HostInheritId = -2;

        protected InnerNetObject(Game game)
        {
            Game = game;
        }

        public uint NetId { get; internal set; }

        public int OwnerId { get; internal set; }

        public Game Game { get; }

        IGame IInnerNetObject.Game => Game;

        public SpawnFlags SpawnFlags { get; internal set; }

        public bool IsOwnedBy(IClientPlayer player)
        {
            return OwnerId == player.Client.Id ||
                   (OwnerId == HostInheritId && player.IsHost);
        }

        public abstract ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState);

        public abstract ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState);

        public abstract ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader);

        public virtual ValueTask OnSpawnAsync()
        {
            return default;
        }
    }
}
