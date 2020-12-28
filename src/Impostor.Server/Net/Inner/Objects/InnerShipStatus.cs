using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Server.Events.Ship;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerShipStatus : InnerNetObject
    {
        private readonly ILogger<InnerShipStatus> _logger;
        private readonly IEventManager _eventManager;
        private readonly Game _game;
        private readonly Dictionary<SystemTypes, ISystemType> _systems;

        public InnerShipStatus(ILogger<InnerShipStatus> logger, IEventManager eventManager, Game game)
        {
            _logger = logger;
            _eventManager = eventManager;
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

        public override async ValueTask HandleRpc(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.CloseDoorsOfType:
                {
                    if (target == null || !target.IsHost)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.CloseDoorsOfType)} to wrong destinition, must be host");
                    }

                    if (!sender.Character.PlayerInfo.IsImpostor)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.CloseDoorsOfType)} as crewmate");
                    }

                    var systemType = (SystemTypes)reader.ReadByte();
                    
                    await _eventManager.CallAsync(new ShipDoorsCloseEvent(_game, this, sender, systemType));
                    break;
                }

                case RpcCalls.RepairSystem:
                {
                    if (target == null || !target.IsHost)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.RepairSystem)} to wrong destinition, must be host");
                    }
                    
                    var systemType = (SystemTypes)reader.ReadByte();
                    if (systemType == SystemTypes.Sabotage && !sender.Character.PlayerInfo.IsImpostor)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.RepairSystem)} for {systemType} as crewmate");
                    }

                    var player = reader.ReadNetObject<InnerPlayerControl>(_game);
                    var flag = reader.ReadByte();
                    
                    Console.WriteLine(systemType + " " + flag);
                    Console.WriteLine(player.NetId);
                    
                    switch (systemType)
                    {
                        case SystemTypes.Sabotage:

                            if (!new List<SystemTypes>
                            {
                                SystemTypes.Reactor, SystemTypes.Electrical, SystemTypes.LifeSupp, SystemTypes.Comms, SystemTypes.Laboratory
                            }.Contains((SystemTypes)flag))
                            {
                                _logger.LogWarning("{0}: Unknown sabotage type {1}", nameof(InnerShipStatus), flag);
                                break;
                            }
                            
                            await _eventManager.CallAsync(new ShipSabotageEvent(_game, this, sender, (SystemTypes)flag));
                            break;
                        
                        case SystemTypes.Doors:

                            if (_game.Options.MapId != 2)
                            {
                                _logger.LogWarning($"{nameof(InnerShipStatus)}: Client sent {nameof(RpcCalls.RepairSystem)} for {nameof(SystemTypes.Doors)} on map {_game.Options.MapId}");
                                break;
                            }
                            
                            if (flag < 64 || flag > 75)
                            {
                                _logger.LogWarning("{0}: Unknown polus door {1}", nameof(InnerShipStatus), flag);
                                break;
                            }
                            
                            await _eventManager.CallAsync(new ShipPolusDoorOpenEvent(_game, this, sender, (PolusDoors)flag));

                            break;
                        
                        case SystemTypes.Decontamination:
                        case SystemTypes.TopDecontaminationPolus:

                            if (_game.Options.MapId == 0)
                            {
                                _logger.LogWarning($"{nameof(InnerShipStatus)}: Client sent {nameof(RpcCalls.RepairSystem)} for {systemType} on map {_game.Options.MapId}");
                                break;
                            }
                            
                            if (flag < 1 || flag > 4)
                            {
                                _logger.LogWarning("{0}: Unknown decontamination door {1}", nameof(InnerShipStatus), flag);
                                break;
                            }

                            await _eventManager.CallAsync(new ShipDecontamDoorOpenEvent(_game, this, sender, systemType, flag));
                            
                            break;
                    }
                    
                    break;
                }

                default:
                {
                    _logger.LogWarning("{0}: Unknown rpc call {1}", nameof(InnerShipStatus), call);
                    break;
                }
            }
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

            if (target != null)
            {
                throw new ImpostorCheatException($"Client attempted to send {nameof(InnerShipStatus)} data to a specific player, must be broadcast");
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

            return ValueTask.CompletedTask;
        }
    }
}
