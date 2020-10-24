using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net
{
    public abstract class InnerNetObject : GameObject
    {
        private const int HostInheritId = -2;

        public uint NetId { get; internal set; }

        public int OwnerId { get; internal set; }

        public SpawnFlags SpawnFlags { get; internal set; }

        public abstract ValueTask HandleRpc(IClientPlayer sender, IClientPlayer? target, RpcCalls call, IMessageReader reader);

        public abstract bool Serialize(IMessageWriter writer, bool initialState);

        public abstract void Deserialize(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState);

        public bool IsOwnedBy(IClientPlayer player)
        {
            return OwnerId == player.Client.Id ||
                   (OwnerId == HostInheritId && player.IsHost);
        }
    }
}