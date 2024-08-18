using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Api.Utils;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerInfo
    {
        private readonly IEventManager _eventManager;
        private readonly ILogger<InnerPlayerInfo> _logger;

        public InnerPlayerInfo(ICustomMessageManager<ICustomRpc> customMessageManager, IEventManager eventManager, Game game, ILogger<InnerPlayerInfo> logger) : base(customMessageManager, game)
        {
            Components.Add(this);
            _eventManager = eventManager;
            _logger = logger;
        }

        public InnerPlayerControl? Controller { get; internal set; }

        public byte PlayerId { get; internal set; }

        public int ClientId { get; internal set; }

        public string PlayerName => CurrentOutfit.PlayerName;

        public Dictionary<PlayerOutfitType, PlayerOutfit> Outfits { get; } = new()
        {
            [PlayerOutfitType.Default] = new PlayerOutfit(),
        };

        public PlayerOutfitType CurrentOutfitType { get; set; } = PlayerOutfitType.Default;

        public PlayerOutfit CurrentOutfit => Outfits[CurrentOutfitType];

        public RoleTypes? RoleType { get; internal set; }

        public RoleTypes? RoleWhenAlive { get; internal set; }

        public bool Disconnected { get; internal set; }

        public bool IsImpostor => RoleType is RoleTypes.Impostor or RoleTypes.Shapeshifter or RoleTypes.ImpostorGhost or RoleTypes.Phantom;

        public bool CanVent => RoleType is RoleTypes.Impostor or RoleTypes.Shapeshifter or RoleTypes.Engineer;

        public bool IsDead { get; internal set; }

        public DeathReason LastDeathReason { get; internal set; }

        public List<TaskInfo> Tasks { get; internal set; } = new List<TaskInfo>(0);

        public DateTimeOffset LastMurder { get; set; }

        public uint PlayerLevel { get; internal set; }

        public bool CanMurder(IGame game, IDateTimeProvider dateTimeProvider)
        {
            if (!IsImpostor)
            {
                return false;
            }

            // In 2021.11.9 the Guardian Angel was created, which was unfortunately implemented in such a way that
            // it is hard to determine accurately whether a kill was prevented or not. When a kill was prevented,
            // the impostor has a cooldown of half the usual duration. As a workaround we always assume the kill was
            // prevented and the impostor only has half of its cooldown.
            // FIXME when the base game improved their implementation.
            if (game.Options.GameMode is GameModes.Normal or GameModes.NormalFools)
            {
                var options = (NormalGameOptions)game.Options;
                return dateTimeProvider.UtcNow.Subtract(LastMurder).TotalSeconds >= options.KillCooldown / 2;
            }
            else
            {
                return true;
            }
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            writer.Write(PlayerId);
            writer.WritePacked(ClientId);

            writer.Write((byte)Outfits.Count);
            foreach (var outfit in Outfits)
            {
                writer.Write((byte)outfit.Key);
                outfit.Value.Serialize(writer);
            }

            writer.WritePacked(PlayerLevel);

            var flag = 0;
            if (Disconnected)
            {
                flag = (byte)(flag | 1u);
            }

            if (IsDead)
            {
                flag = (byte)(flag | 4u);
            }

            writer.Write((byte)flag);

            writer.Write((ushort)(RoleType ?? 0));
            writer.Write(RoleWhenAlive.HasValue);
            if (RoleWhenAlive.HasValue)
            {
                writer.Write((ushort)RoleWhenAlive.Value);
            }

            writer.Write((byte)Tasks.Count);
            for (var i = 0; i < Tasks.Count; i++)
            {
                Tasks[i].Serialize(writer);
            }

            writer.Write(string.Empty); // FriendCode
            writer.Write(string.Empty); // PUID
            return new ValueTask<bool>(true);
        }

        public override ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            PlayerId = reader.ReadByte();
            ClientId = reader.ReadPackedInt32();

            Outfits.Clear();
            var b = reader.ReadByte();
            for (var i = 0; i < b; i++)
            {
                var key = (PlayerOutfitType)reader.ReadByte();
                Outfits[key] = new PlayerOutfit();
                Outfits[key].Deserialize(reader);
            }

            PlayerLevel = reader.ReadPackedUInt32();

            var flag = reader.ReadByte();
            Disconnected = (flag & 1) != 0;
            IsDead = (flag & 4) != 0;

            // Ignore the RoleType here and only trust the SetRole RPC, as
            // RoleType is not nullable in vanilla, while Impostor checks game
            // starts based on assigned roles.
            _ = (RoleTypes)reader.ReadUInt16();

            if (reader.ReadBoolean())
            {
                RoleWhenAlive = (RoleTypes)reader.ReadUInt16();
            }

            var taskCount = reader.ReadByte();

            if (Tasks.Count < taskCount)
            {
                taskCount = (byte)Tasks.Count;
            }

            for (var i = 0; i < taskCount; i++)
            {
                Tasks[i].Deserialize(reader);
            }

            // Impostor doesn't expose fields that aren't properly validated
            reader.ReadString(); // FriendCode
            reader.ReadString(); // PUID

            return ValueTask.CompletedTask;
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.SetTasks:
                    Rpc29SetTasks.Deserialize(reader, out var taskTypeIds);
                    SetTasks(taskTypeIds);
                    break;

                default:
                    return await base.HandleRpcAsync(sender, target, call, reader);
            }

            return true;
        }

        private void SetTasks(ReadOnlyMemory<byte> taskTypeIds)
        {
            if (Disconnected)
            {
                return;
            }

            Tasks = new List<TaskInfo>(taskTypeIds.Length);

            var taskId = 0u;
            foreach (var taskTypeId in taskTypeIds.Span)
            {
                var mapTasks = Game.GameNet!.ShipStatus?.Data.Tasks;
                var taskType = (mapTasks != null && mapTasks.ContainsKey(taskTypeId)) ? mapTasks[taskTypeId] : null;
                Tasks.Add(new TaskInfo(this, _eventManager, taskId++, taskType));
            }
        }
    }
}
