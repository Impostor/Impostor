using System;
using System.Collections.Generic;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net.Objects.Systems;
using Impostor.Api.Innersloth.Net.Objects.Systems.ShipStatus;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public class InnerShipStatus : InnerNetObject
    {
        private readonly IGame _game;
        private readonly Dictionary<SystemTypes, ISystemType> _systems;

        public InnerShipStatus(IGame game)
        {
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

        public override void HandleRpc(IClientPlayer sender, byte callId, IMessageReader reader)
        {
            switch (callId)
            {
                case 27:
                {
                    if (!sender.Character.PlayerInfo.IsImpostor)
                    {
                        Console.WriteLine($"OOPS Fake Impostor: {sender.Character.PlayerInfo.PlayerName} did doors");
                    }

                    break;
                }

                case 28:
                {
                    var systemType = (SystemTypes)reader.ReadByte();
                    var player = _game.FindObjectByNetId<InnerPlayerControl>(reader.ReadPackedUInt32());
                    var amount = reader.ReadByte();

                    if (systemType == SystemTypes.Sabotage && !player.PlayerInfo.IsImpostor)
                    {
                        Console.WriteLine($"OOPS Fake Impostor: {player.PlayerInfo.PlayerName} did {(SystemTypes) amount}");
                    }

                    break;
                }
            }
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IMessageReader reader, bool initialState)
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