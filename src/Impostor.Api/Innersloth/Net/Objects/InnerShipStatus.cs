using System;
using System.Collections.Generic;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net.Objects.Systems;
using Impostor.Api.Innersloth.Net.Objects.Systems.ShipStatus;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.Logging;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public class InnerShipStatus : InnerNetObject
    {
        private readonly ILogger<InnerShipStatus> _logger;
        private readonly IGame _game;
        private readonly Dictionary<SystemTypes, ISystemType> _systems;

        public InnerShipStatus(ILogger<InnerShipStatus> logger, IGame game)
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
                (IActivatable)_systems[SystemTypes.Comms],
                (IActivatable)_systems[SystemTypes.Reactor],
                (IActivatable)_systems[SystemTypes.LifeSupp],
                (IActivatable)_systems[SystemTypes.Electrical],
            }));

            Components.Add(this);
        }

        public override void HandleRpc(IClientPlayer sender, IClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.CloseDoorsOfType:
                {
                    if (target == null || !target.IsHost)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.CloseDoorsOfType)} to wrong destinition, must be host.");
                    }

                    if (!sender.Character.PlayerInfo.IsImpostor)
                    {
                        // throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.CloseDoorsOfType)} as crewmate.");
                    }

                    var systemType = (SystemTypes)reader.ReadByte();

                    break;
                }

                case RpcCalls.RepairSystem:
                {
                    if (target == null || !target.IsHost)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.RepairSystem)} to wrong destinition, must be host.");
                    }

                    if (!sender.Character.PlayerInfo.IsImpostor)
                    {
                        // throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.RepairSystem)} as crewmate.");
                    }

                    var systemType = (SystemTypes)reader.ReadByte();
                    var player = reader.ReadNetObject<InnerPlayerControl>(_game);
                    var amount = reader.ReadByte();

                    // TODO: Modify data (?)

                    break;
                }

                default:
                {
                    _logger.LogWarning("{0}: Unknown rpc call {1}", nameof(InnerShipStatus), call);
                    break;
                }
            }
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
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
    }
}