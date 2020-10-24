using System;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.Logging;

namespace Impostor.Api.Innersloth.Net.Objects.Components
{
    public class InnerPlayerPhysics : InnerNetObject
    {
        private readonly ILogger<InnerPlayerPhysics> _logger;

        public InnerPlayerPhysics(ILogger<InnerPlayerPhysics> logger)
        {
            _logger = logger;
        }

        public override void HandleRpc(IClientPlayer sender, IClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (call != RpcCalls.EnterVent && call != RpcCalls.ExitVent)
            {
                _logger.LogWarning("{0}: Unknown rpc call {1}", nameof(InnerPlayerPhysics), call);
                return;
            }

            if (!sender.IsOwner(this))
            {
                throw new ImpostorCheatException($"Client sent {call} to an unowned {nameof(InnerPlayerControl)}");
            }

            if (target != null)
            {
                throw new ImpostorCheatException($"Client sent {call} to a specific player instead of broadcast");
            }

            if (!sender.Character.PlayerInfo.IsImpostor)
            {
                throw new ImpostorCheatException($"Client sent {call} as crewmate");
            }

            var ventId = reader.ReadPackedUInt32();
            var ventEnter = call == RpcCalls.EnterVent;

            // TODO: Do stuff.
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            throw new NotImplementedException();
        }
    }
}