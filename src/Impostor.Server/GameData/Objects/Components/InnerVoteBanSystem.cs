using System;
using System.Collections.Generic;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.GameData.Objects.Components
{
    public class InnerVoteBanSystem : InnerNetObject
    {
        private readonly Dictionary<int, int[]> _votes;

        public InnerVoteBanSystem()
        {
            _votes = new Dictionary<int, int[]>();
        }

        public override void HandleRpc(byte callId, IMessageReader reader)
        {
            throw new NotImplementedException();
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IMessageReader reader, bool initialState)
        {
            var unknown = reader.ReadBoolean();
            if (unknown)
            {
                while (true)
                {
                    var v4 = reader.ReadInt32();

                    if (!_votes.TryGetValue(v4, out var v12))
                    {
                        v12 = new int[3];
                        _votes[v4] = v12;
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        v12[i] = reader.ReadPackedInt32();
                    }
                }
            }
        }
    }
}