using System;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Events.Player;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.Components
{
    internal partial class InnerPlayerPhysics : InnerNetObject
    {
        private readonly ILogger<InnerPlayerPhysics> _logger;
        private readonly InnerPlayerControl _playerControl;
        private readonly IEventManager _eventManager;

        public InnerPlayerPhysics(Game game, ILogger<InnerPlayerPhysics> logger, InnerPlayerControl playerControl, IEventManager eventManager) : base(game)
        {
            _logger = logger;
            _playerControl = playerControl;
            _eventManager = eventManager;
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (!await ValidateOwnership(call, sender))
            {
                return false;
            }

            switch (call)
            {
                case RpcCalls.EnterVent:
                case RpcCalls.ExitVent:
                    if (!await ValidateImpostor(call, sender, _playerControl.PlayerInfo))
                    {
                        return false;
                    }

                    int ventId;

                    switch (call)
                    {
                        case RpcCalls.EnterVent:
                            Rpc19EnterVent.Deserialize(reader, out ventId);
                            break;
                        case RpcCalls.ExitVent:
                            Rpc20ExitVent.Deserialize(reader, out ventId);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(call), call, null);
                    }

                    await _eventManager.CallAsync(new PlayerVentEvent(Game, sender, _playerControl, (VentLocation)ventId, call == RpcCalls.EnterVent));
                    break;

                case RpcCalls.ClimbLadder:
                    Rpc31ClimbLadder.Deserialize(reader, out var ladderId, out var lastClimbLadderSid);
                    break;

                default:
                    return await UnregisteredCall(call, sender);
            }

            return true;
        }
    }
}
