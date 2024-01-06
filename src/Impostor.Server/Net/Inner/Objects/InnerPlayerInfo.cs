﻿using System;
using System.Collections.Generic;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Utils;

namespace Impostor.Server.Net.Inner.Objects;

internal partial class InnerPlayerInfo
{
    public InnerPlayerInfo(byte playerId)
    {
        PlayerId = playerId;
    }

    public InnerPlayerControl? Controller { get; internal set; }

    public byte PlayerId { get; }

    public bool Disconnected { get; internal set; }

    public bool CanVent => RoleType is RoleTypes.Impostor or RoleTypes.Shapeshifter or RoleTypes.Engineer;

    public List<InnerGameData.TaskInfo> Tasks { get; internal set; } = new(0);

    public string PlayerName { get; internal set; } = string.Empty;

    public Dictionary<PlayerOutfitType, PlayerOutfit> Outfits { get; } = new()
    {
        [PlayerOutfitType.Default] = new PlayerOutfit(),
    };

    public PlayerOutfitType CurrentOutfitType { get; set; } = PlayerOutfitType.Default;

    public PlayerOutfit CurrentOutfit => Outfits[CurrentOutfitType];

    public RoleTypes? RoleType { get; internal set; }

    public bool IsImpostor => RoleType is RoleTypes.Impostor or RoleTypes.Shapeshifter or RoleTypes.ImpostorGhost;

    public bool IsDead { get; internal set; }

    public DeathReason LastDeathReason { get; internal set; }

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
        if (game.Options.GameMode == GameModes.Normal)
        {
            var options = (NormalGameOptions)game.Options;
            return dateTimeProvider.UtcNow.Subtract(LastMurder).TotalSeconds >= options.KillCooldown / 2;
        }

        return true;
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

        PlayerLevel = reader.ReadPackedUInt32();

        var flag = reader.ReadByte();
        Disconnected = (flag & 1) != 0;
        IsDead = (flag & 4) != 0;

        _ = (RoleTypes)reader.ReadUInt16(); // ignore the RoleType here and only trust the SetRole rpc

        var taskCount = reader.ReadByte();

        if (Tasks.Count < taskCount)
        {
            taskCount = (byte)Tasks.Count;
        }

        for (var i = 0; i < taskCount; i++)
        {
            Tasks[i].Deserialize(reader);
        }
    }
}
