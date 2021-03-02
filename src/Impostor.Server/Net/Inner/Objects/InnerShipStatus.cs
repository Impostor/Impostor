using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects
{
    internal class InnerShipStatus : InnerNetObject, IInnerShipStatus
    {
        private readonly ILogger<InnerShipStatus> _logger;
        private readonly Game _game;
        private readonly Dictionary<SystemTypes, ISystemType> _systems;

        public InnerShipStatus(ILogger<InnerShipStatus> logger, Game game)
        {
            _logger = logger;
            _game = game;

            _systems = new Dictionary<SystemTypes, ISystemType>
            {
                [SystemTypes.Electrical] = new SwitchSystem(),
                [SystemTypes.MedBay] = new MedScanSystem(),
                [SystemTypes.Reactor] = new ReactorSystemType(),
                [SystemTypes.LifeSupp] = new LifeSuppSystemType(),
                [SystemTypes.Security] = new SecurityCameraSystemType(),
                [SystemTypes.Comms] = new HudOverrideSystemType(),
                [SystemTypes.Doors] = new DoorsSystemType(_game),
            };

            _systems.Add(SystemTypes.Sabotage, new SabotageSystemType(new[]
            {
                (IActivatable)_systems[SystemTypes.Comms], (IActivatable)_systems[SystemTypes.Reactor], (IActivatable)_systems[SystemTypes.LifeSupp], (IActivatable)_systems[SystemTypes.Electrical],
            }));

            Components.Add(this);
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            if (!await ValidateHost(CheatContext.Deserialize, sender) || !await ValidateBroadcast(CheatContext.Deserialize, sender, target))
            {
                return;
            }

            if (initialState)
            {
                // TODO: (_systems[SystemTypes.Doors] as DoorsSystemType).SetDoors();
                foreach (var systemType in SystemTypeHelpers.AllTypes)
                {
                    if (_systems.TryGetValue(systemType, out var system))
                    {
                        system.Deserialize(reader, true);
                    }
                }
            }
            else
            {
                var count = reader.ReadPackedUInt32();

                foreach (var systemType in SystemTypeHelpers.AllTypes)
                {
                    // TODO: Not sure what is going on here, check.
                    if ((count & 1 << (int)(systemType & (SystemTypes.ShipTasks | SystemTypes.Doors))) != 0L)
                    {
                        if (_systems.TryGetValue(systemType, out var system))
                        {
                            system.Deserialize(reader, false);
                        }
                    }
                }
            }
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (!await ValidateCmd(call, sender, target))
            {
                return false;
            }

            switch (call)
            {
                case RpcCalls.CloseDoorsOfType:
                {
                    if (!await ValidateImpostor(RpcCalls.MurderPlayer, sender, sender.Character!.PlayerInfo))
                    {
                        return false;
                    }

                    Rpc27CloseDoorsOfType.Deserialize(reader, out var systemType);
                    break;
                }

                case RpcCalls.RepairSystem:
                {
                    Rpc28RepairSystem.Deserialize(reader, _game, out var systemType, out var player, out var amount);

                    if (systemType == SystemTypes.Sabotage && !await ValidateImpostor(call, sender, sender.Character!.PlayerInfo))
                    {
                        return false;
                    }

                    break;
                }

                case RpcCalls.CustomRpc:
                    return await HandleCustomRpc(reader, _game);

                default:
                    return await UnregisteredCall(call, sender);
            }

            return true;
        }
    }
}
