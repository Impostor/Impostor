using System;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
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
        private readonly Game _game;

        public InnerPlayerPhysics(ILogger<InnerPlayerPhysics> logger, InnerPlayerControl playerControl, IEventManager eventManager, Game game)
        {
            _logger = logger;
            _playerControl = playerControl;
            _eventManager = eventManager;
            _game = game;
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
            if (!await ValidateOwnership(call, sender) || !await ValidateImpostor(RpcCalls.MurderPlayer, sender, _playerControl.PlayerInfo))
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
                    Rpc19EnterVent.Deserialize(reader, out ventId);
                    break;

                case RpcCalls.CustomRpc:
                    return await HandleCustomRpc(reader, _game);

                default:
                    return await UnregisteredCall(call, sender);
            }

            await _eventManager.CallAsync(new PlayerVentEvent(_game, sender, _playerControl, (VentLocation)ventId, call == RpcCalls.EnterVent));

            return true;
        }
    }
}
