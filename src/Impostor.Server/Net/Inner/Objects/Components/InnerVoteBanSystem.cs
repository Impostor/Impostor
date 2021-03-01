using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal class InnerVoteBanSystem : InnerNetObject, IInnerVoteBanSystem
    {
        private readonly ILogger<InnerVoteBanSystem> _logger;
        private readonly Dictionary<int, int[]> _votes;

        private static Dictionary<RpcCalls, RpcInfo> Rpcs { get; } = new Dictionary<RpcCalls, RpcInfo>
        {
            [RpcCalls.AddVote] = new RpcInfo(),
        };

        public InnerVoteBanSystem(ILogger<InnerVoteBanSystem> logger)
        {
            _logger = logger;
            _votes = new Dictionary<int, int[]>();
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            if (!sender.IsHost)
            {
                if (await sender.Client.ReportCheatAsync(CheatContext.Deserialize, $"Client attempted to send data for {nameof(InnerShipStatus)} as non-host"))
                {
                    return;
                }
            }

            var votes = _votes;
            var unknown = reader.ReadByte();
            if (unknown != 0)
            {
                for (var i = 0; i < unknown; i++)
                {
                    var v4 = reader.ReadInt32();
                    if (v4 == 0)
                    {
                        break;
                    }

                    if (!votes.TryGetValue(v4, out var v12))
                    {
                        v12 = new int[3];
                        votes[v4] = v12;
                    }

                    for (var j = 0; j < 3; j++)
                    {
                        v12[j] = reader.ReadPackedInt32();
                    }
                }
            }
        }

        public override async ValueTask<bool> HandleRpc(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (!await TestRpc(sender, target, call, Rpcs))
            {
                return false;
            }

            if (call == RpcCalls.AddVote)
            {
                Rpc26AddVote.Deserialize(reader, out var clientId, out var targetClientId);

                if (clientId != sender.Client.Id)
                {
                    if (await sender.Client.ReportCheatAsync(RpcCalls.AddVote, $"Client sent {nameof(RpcCalls.AddVote)} as other client"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
