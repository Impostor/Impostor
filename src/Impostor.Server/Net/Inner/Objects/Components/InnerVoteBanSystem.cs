using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal class InnerVoteBanSystem : InnerNetObject, IInnerVoteBanSystem
    {
        private readonly ILogger<InnerVoteBanSystem> _logger;
        private readonly Dictionary<int, int[]> _votes;

        public InnerVoteBanSystem(ILogger<InnerVoteBanSystem> logger)
        {
            _logger = logger;
            _votes = new Dictionary<int, int[]>();
        }

        public override ValueTask HandleRpc(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (call != RpcCalls.AddVote)
            {
                _logger.LogWarning("{0}: Unknown rpc call {1}", nameof(InnerVoteBanSystem), call);
                return default;
            }

            var clientId = reader.ReadInt32();
            if (clientId != sender.Client.Id)
            {
                throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.AddVote)} as other client");
            }

            if (target != null)
            {
                throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.CastVote)} to wrong destinition, must be broadcast");
            }

            var targetClientId = reader.ReadInt32();

            // TODO: Use.

            return default;
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            if (!sender.IsHost)
            {
                throw new ImpostorCheatException($"Client attempted to send data for {nameof(InnerShipStatus)} as non-host");
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

            return ValueTask.CompletedTask;
        }
    }
}
