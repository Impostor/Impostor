using System;
using System.Collections.Generic;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net.Messages;
using Impostor.Api.Utils;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerInfo
    {
        public InnerPlayerInfo(byte playerId)
        {
            PlayerId = playerId;
        }

        public InnerPlayerControl? Controller { get; internal set; }

        public byte PlayerId { get; }

        public string PlayerName { get; internal set; } = string.Empty;

        public Dictionary<PlayerOutfitType, PlayerOutfit> Outfits { get; } = new()
        {
            [PlayerOutfitType.Default] = new PlayerOutfit(),
        };

        public PlayerOutfitType CurrentOutfitType { get; set; } = PlayerOutfitType.Default;

        public PlayerOutfit CurrentOutfit => Outfits[CurrentOutfitType];

        public RoleTypes RoleType { get; internal set; }

        public bool Disconnected { get; internal set; }

        public bool IsImpostor => RoleType is RoleTypes.Impostor or RoleTypes.Shapeshifter;

        public bool IsDead { get; internal set; }

        public DeathReason LastDeathReason { get; internal set; }

        public List<InnerGameData.TaskInfo> Tasks { get; internal set; } = new List<InnerGameData.TaskInfo>(0);

        public DateTimeOffset LastMurder { get; set; }

        public bool CanMurder(IGame game, IDateTimeProvider dateTimeProvider)
        {
            if (!IsImpostor)
            {
                return false;
            }

            return dateTimeProvider.UtcNow.Subtract(LastMurder).TotalSeconds >= game.Options.KillCooldown;
        }

        public void Serialize(IMessageWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(IMessageReader reader)
        {
            Outfits.Clear();
            var b = reader.ReadByte();
            for (var i = 0; i < b; i++)
            {
                var key = (PlayerOutfitType)reader.ReadByte();
                Outfits[key] = new PlayerOutfit();
                Outfits[key].Deserialize(reader);
            }

            var PlayerLevel = reader.ReadPackedUInt32();

            var flag = reader.ReadByte();
            Disconnected = (flag & 1) != 0;
            IsDead = (flag & 4) != 0;

            var roleType = (RoleTypes)reader.ReadUInt16();
            // Role = roleType;

            var taskCount = reader.ReadByte();

            if (Tasks.Count != taskCount)
            {
                Tasks = new List<InnerGameData.TaskInfo>(taskCount);
            }

            for (var i = 0; i < taskCount; i++)
            {
                Tasks[i].Deserialize(reader);
            }
        }
    }
}
