using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Utils;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerInfo
    {
        public InnerPlayerInfo(ICustomMessageManager<ICustomRpc> customMessageManager, Game game) : base(customMessageManager, game)
        {
            Components.Add(this);
        }

        public InnerPlayerControl? Controller { get; internal set; }

        public byte PlayerId { get; internal set; }

        public int ClientId { get; internal set; }

        public string PlayerName { get; internal set; } = string.Empty;

        public Dictionary<PlayerOutfitType, PlayerOutfit> Outfits { get; } = new()
        {
            [PlayerOutfitType.Default] = new PlayerOutfit(),
        };

        public PlayerOutfitType CurrentOutfitType { get; set; } = PlayerOutfitType.Default;

        public PlayerOutfit CurrentOutfit => Outfits[CurrentOutfitType];

        public RoleTypes? RoleType { get; internal set; }

        public RoleTypes? RoleWhenAlive { get; internal set; }

        public bool Disconnected { get; internal set; }

        public bool IsImpostor => RoleType is RoleTypes.Impostor or RoleTypes.Shapeshifter or RoleTypes.ImpostorGhost;

        public bool CanVent => RoleType is RoleTypes.Impostor or RoleTypes.Shapeshifter or RoleTypes.Engineer;

        public bool IsDead { get; internal set; }

        public DeathReason LastDeathReason { get; internal set; }

        public List<InnerGameData.TaskInfo> Tasks { get; internal set; } = new List<InnerGameData.TaskInfo>(0);

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
            foreach (KeyValuePair<PlayerOutfitType, PlayerOutfit> outfit in Outfits)
            {
                writer.Write((byte)outfit.Key);
                outfit.Value.Serialize(writer);
            }

            writer.WritePacked(PlayerLevel);

            byte flag = 0;
            if (Disconnected)
            {
                flag = (byte)(flag | 1u);
            }
            if (IsDead)
            {
                flag = (byte)(flag | 4u);
            }
            writer.Write(flag);

            writer.Write((ushort)(RoleType ?? 0));
            writer.Write(RoleWhenAlive.HasValue);
            if (RoleWhenAlive.HasValue)
            {
                writer.Write((ushort)RoleWhenAlive.Value);
            }

            writer.Write((byte)Tasks.Count);
            for (int i = 0; i < Tasks.Count; i++)
            {
                Tasks[i].Serialize(writer);
            }
            writer.Write(string.Empty);
            writer.Write(string.Empty);
            return new ValueTask<bool>(true);
        }

        public override ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            // TODO ac

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

            RoleType = (RoleTypes)reader.ReadUInt16(); // TODO ignore the RoleType here and only trust the SetRole rpc?

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
            reader.ReadString();
            reader.ReadString();

            return ValueTask.CompletedTask;
        }
    }
}
