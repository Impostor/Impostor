using System;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal class InnerPlayerPhysics : InnerNetObject
    {
        private readonly ILogger<InnerPlayerPhysics> _logger;

        public InnerPlayerPhysics(ILogger<InnerPlayerPhysics> logger)
        {
            _logger = logger;
        }

        public override ValueTask HandleRpc(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (call != RpcCalls.EnterVent && call != RpcCalls.ExitVent)
            {
                _logger.LogWarning("{0}: Unknown rpc call {1}", nameof(InnerPlayerPhysics), call);
                return default;
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

            return default;
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