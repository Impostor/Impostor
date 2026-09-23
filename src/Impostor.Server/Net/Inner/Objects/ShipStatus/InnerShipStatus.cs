using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Maps;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.ShipStatus;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.ShipStatus
{
    internal abstract class InnerShipStatus : InnerNetObject, IInnerShipStatus
    {
        private readonly ICustomMessageManager<ICustomSystemType> _customSystemManager;
        private readonly Dictionary<SystemTypes, ISystemType> _systems = new Dictionary<SystemTypes, ISystemType>();

        protected InnerShipStatus(ICustomMessageManager<ICustomRpc> customMessageManager, ICustomMessageManager<ICustomSystemType> customSystemManager, Game game, MapTypes mapType) : base(customMessageManager, game)
        {
            _customSystemManager = customSystemManager;

            Components.Add(this);

            MapType = mapType;
            Data = MapData.Maps[mapType];
            Doors = new Dictionary<int, bool>(Data.Doors.Count);
        }

        public MapTypes MapType { get; }

        public MapData Data { get; }

        public Dictionary<int, bool> Doors { get; }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            var result = false;
            foreach (var systemType in SystemTypeHelpers.AllTypes)
            {
                if (_systems.TryGetValue(systemType, out var value))
                {
                    result = true;
                    writer.StartMessage((byte)systemType);
                    value.Serialize(writer, initialState);
                    writer.EndMessage();
                }
            }

            return new ValueTask<bool>(result);
        }

        public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            if (!await ValidateHost(CheatContext.Deserialize, sender) || !await ValidateBroadcast(CheatContext.Deserialize, sender, target))
            {
                return;
            }

            while (reader.Position < reader.Length)
            {
                using var messageReader = reader.ReadMessage();
                var type = (SystemTypes)messageReader.Tag;
                if (_systems.TryGetValue(type, out var value))
                {
                    value.Deserialize(messageReader, initialState);
                }
                else if (_customSystemManager.TryGet(messageReader.Tag, out var customSystem))
                {
                    await customSystem.DeserializeAsync(this, sender, target, messageReader, initialState);
                }
            }
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.CloseDoorsOfType:
                {
                    if (!await ValidateCmd(call, sender, target) ||
                        !await ValidateImpostor(call, sender, sender.Character!.PlayerInfo))
                    {
                        return false;
                    }

                    Rpc27CloseDoorsOfType.Deserialize(reader, out var systemType);
                    CloseDoorsOfType(systemType);
                    break;
                }

                case RpcCalls.UpdateSystem:
                {
                    if (!await ValidateCmd(call, sender, target))
                    {
                        return false;
                    }

                    Rpc35UpdateSystem.Deserialize(reader, Game, out var systemType, out var playerControl, out var payload);
                    if (_systems.TryGetValue(systemType, out var system))
                    {
                        system.UpdateSystem(playerControl, payload);
                    }
                    else if (_customSystemManager.TryGet((byte)systemType, out var customSystem))
                    {
                        return await customSystem.HandleUpdateSystemAsync(this, playerControl, sender, target, payload);
                    }
                    else if (await sender.Client.ReportCheatAsync(call, CheatCategory.ProtocolExtension, $"Client sent unregistered system type {(byte)systemType}"))
                    {
                        return false;
                    }

                    break;
                }

                default:
                    return await base.HandleRpcAsync(sender, target, call, reader);
            }

            return true;
        }

        public virtual Vector2 GetSpawnLocation(InnerPlayerControl player, int numPlayers, bool initialSpawn)
        {
            var vector = new Vector2(0, 1);
            vector = Rotate(vector, (player.PlayerId - 1) * (360f / numPlayers));
            vector *= Data.SpawnRadius;
            return (initialSpawn ? Data.InitialSpawnCenter : Data.MeetingSpawnCenter) + vector + new Vector2(0f, 0.3636f);
        }

        protected virtual void AddSystems(Dictionary<SystemTypes, ISystemType> systems)
        {
            systems.Add(SystemTypes.Electrical, new SwitchSystem());
            systems.Add(SystemTypes.MedBay, new MedScanSystem());
        }

        protected void InitializeSystems()
        {
            for (var i = 0; i < Data.Doors.Count; i++)
            {
                Doors[i] = true;
            }

            AddSystems(_systems);
            _systems.Add(SystemTypes.Sabotage, new SabotageSystemType(_systems.Values.OfType<IActivatable>().ToArray(), UpdateSystem));
        }

        protected void UpdateSystem(IInnerPlayerControl? playerControl, SystemTypes systemType, byte amount)
        {
            if (!_systems.TryGetValue(systemType, out var system))
            {
                return;
            }

            switch (system)
            {
                case SwitchSystem switchSystem:
                    switchSystem.UpdateSystem(playerControl, amount);
                    break;
                case ReactorSystemType reactorSystem:
                    reactorSystem.UpdateSystem(playerControl, amount);
                    break;
                case LifeSuppSystemType lifeSuppSystem:
                    lifeSuppSystem.UpdateSystem(playerControl, amount);
                    break;
                case HudOverrideSystemType hudOverrideSystem:
                    hudOverrideSystem.UpdateSystem(playerControl, amount);
                    break;
                case HqHudSystemType hqHudSystem:
                    hqHudSystem.UpdateSystem(playerControl, amount);
                    break;
                case HeliSabotageSystemType heliSabotageSystem:
                    heliSabotageSystem.UpdateSystem(playerControl, amount);
                    break;
                case MushroomMixupSabotageSystemType mushroomMixupSabotageSystem:
                    mushroomMixupSabotageSystem.UpdateSystem(playerControl, amount);
                    break;
            }
        }

        protected void CloseDoorsOfType(SystemTypes room)
        {
            if (_systems.TryGetValue(SystemTypes.Doors, out var system) && system is IDoorSystem doorSystem)
            {
                doorSystem.CloseDoorsOfType(room);
            }
        }

        private static Vector2 Rotate(Vector2 self, float degrees)
        {
            var f = 0.017453292f * degrees;
            var cos = MathF.Cos(f);
            var sin = MathF.Sin(f);

            return new Vector2((self.X * cos) - (sin * self.Y), (self.X * sin) + (cos * self.Y));
        }
    }
}
