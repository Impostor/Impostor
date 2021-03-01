using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner
{
    internal abstract class InnerNetObject : GameObject, IInnerNetObject
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

        protected async ValueTask<bool> TestRpc(ClientPlayer sender, ClientPlayer? target, RpcCalls call, Dictionary<RpcCalls, RpcInfo> rpcs)
        {
            if (call == RpcCalls.CustomRpc)
            {
                return true;
            }

            if (rpcs.TryGetValue(call, out var rpc))
            {
                if (rpc.CheckOwnership && !sender.IsOwner(this))
                {
                    if (await sender.Client.ReportCheatAsync(call, $"Client sent {call} to an unowned {GetType().Name}"))
                    {
                        return false;
                    }
                }

                if (rpc.RequireHost && !sender.IsHost)
                {
                    if (await sender.Client.ReportCheatAsync(call, $"Client attempted to send {call} as non-host"))
                    {
                        return false;
                    }
                }

                switch (rpc.TargetType)
                {
                    case RpcTargetType.Target when target == null:
                    {
                        if (await sender.Client.ReportCheatAsync(call, $"Client sent {call} as a broadcast instead to specific player"))
                        {
                            return false;
                        }

                        break;
                    }

                    case RpcTargetType.Broadcast when target != null:
                    {
                        if (await sender.Client.ReportCheatAsync(call, $"Client sent {call} to a specific player instead of broadcast"))
                        {
                            return false;
                        }

                        break;
                    }

                    case RpcTargetType.Cmd when target == null || !target.IsHost:
                    {
                        if (await sender.Client.ReportCheatAsync(call, $"Client sent {call} to the wrong player"))
                        {
                            return false;
                        }

                        break;
                    }

                    case RpcTargetType.Both:
                        break;
                }

                return true;
            }

            if (await sender.Client.ReportCheatAsync(call, "Client sent unregistered call"))
            {
                return false;
            }

            return true;
        }

        public abstract ValueTask<bool> HandleRpc(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader);

        protected ValueTask HandleCustomRpc(IMessageReader reader, Game game)
        {
            var lengthOrShortId = reader.ReadPackedInt32();

            var pluginId = lengthOrShortId < 0
                ? game.Host!.Client.ModIdMap[lengthOrShortId]
                : reader.ReadString(lengthOrShortId);

            var id = reader.ReadPackedInt32();

            // TODO handle custom rpcs

            return default;
        }
    }
}
