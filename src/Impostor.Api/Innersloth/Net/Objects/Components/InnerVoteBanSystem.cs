using System;
using System.Collections.Generic;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects.Components
{
    public class InnerVoteBanSystem : InnerNetObject
    {
        private readonly Dictionary<int, int[]> _votes;

        public InnerVoteBanSystem()
        {
            _votes = new Dictionary<int, int[]>();
        }

        public override void HandleRpc(IClientPlayer sender, IClientPlayer target, RpcCalls call, IMessageReader reader)
        {
            throw new NotImplementedException();
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IMessageReader reader, bool initialState)
        {
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
    }
}