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

        public ColorType Color { get; internal set; } = (ColorType)(-1);

        public HatType Hat { get; internal set; }

        public PetType Pet { get; internal set; }

        public SkinType Skin { get; internal set; }

        public bool Disconnected { get; internal set; }

        public bool IsImpostor { get; internal set; }

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
            PlayerName = reader.ReadString();
            Color = (ColorType)reader.ReadPackedInt32();
            Hat = (HatType)reader.ReadPackedUInt32();
            Pet = (PetType)reader.ReadPackedUInt32();
            Skin = (SkinType)reader.ReadPackedUInt32();

            var flag = reader.ReadByte();
            Disconnected = (flag & 1) > 0;
            IsImpostor = (flag & 2) > 0;
            IsDead = (flag & 4) > 0;

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
