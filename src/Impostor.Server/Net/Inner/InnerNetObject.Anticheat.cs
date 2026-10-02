using System;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Server.Net.Anticheat;
using Impostor.Server.Net.Inner.Objects;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner
{
    internal abstract partial class InnerNetObject
    {
        protected async ValueTask<bool> ValidateOwnership(CheatContext context, IClientPlayer sender)
        {
            if (!sender.IsOwner(this))
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Ownership, $"Failed ownership check on {GetType().Name}"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateHost(CheatContext context, IClientPlayer sender)
        {
            if (!sender.IsHost)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.MustBeHost, "Failed host check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateTarget(CheatContext context, IClientPlayer sender, IClientPlayer? target)
        {
            if (target == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Target, "Failed target check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateBroadcast(CheatContext context, IClientPlayer sender, IClientPlayer? target)
        {
            if (target != null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Target, "Failed broadcast check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCmd(CheatContext context, IClientPlayer sender, IClientPlayer? target)
        {
            if (target == null || !target.IsHost)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Target, "Failed cmd check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateImpostor(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if Impostor, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.IsImpostor != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed impostor check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanVent(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can vent, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanVent != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can vent check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateRole(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, RoleTypes role)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check role, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.RoleType != role)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, $"Failed role = {role} check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanDoTask(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can do tasks, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanDoTasks != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can do tasks check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanKill(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can kill, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanKill != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can kill check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanSabotage(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can sabotage, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanSabotage != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can sabotage check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanShapeshift(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can shapeshift, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanShapeshift != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can shapeshift check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanVanish(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can vanish, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanVanish != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can vanish check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanProtect(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can protect, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanProtect != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can protect check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanOverrule(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can overrule, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanOverrule != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can overrule check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateCanSendPhoto(CheatContext context, IClientPlayer sender, InnerPlayerInfo? playerInfo, bool value = true)
        {
            if (playerInfo == null)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.InvalidObject, "Couldn't check if can send photo, playerInfo not set"))
                {
                    return false;
                }
            }
            else if (playerInfo.CanSendPhoto != value)
            {
                if (await sender.Client.ReportCheatAsync(context, CheatCategory.Role, "Failed can send photo check"))
                {
                    return false;
                }
            }

            return true;
        }

        protected async ValueTask<bool> ValidateMurder(CheatContext context, IClientPlayer sender, byte killerPlayerId, IInnerPlayerControl? target)
        {
            if (!Game.AntiCheat.Config.EnableMurderChecks || target == null)
            {
                return true;
            }

            var timeline = Game.AntiCheat.For(killerPlayerId);

            if (target.PlayerInfo is { IsDead: true })
            {
                timeline.DeadTargetKills++;

                if (timeline.DeadTargetKills >= 6 &&
                    await sender.Client.ReportCheatAsync(
                        context,
                        CheatCategory.Murder,
                        $"Client murdered the already dead player {target.PlayerId} {timeline.DeadTargetKills} times"))
                {
                    return false;
                }
            }
            else
            {
                timeline.DeadTargetKills = 0;
            }

            if (Game.Options is not NormalGameOptions options ||
                Game.Options.GameMode is not (GameModes.Normal or GameModes.NormalFools))
            {
                return true;
            }

            var now = AntiCheatState.Now;
            var window = options.KillCooldown / 2;

            if (window > 0 &&
                timeline.LastKillAt >= 0 &&
                timeline.LastKillTarget != target.PlayerId &&
                now - timeline.LastKillAt < window)
            {
                return await sender.Client.ReportCheatAsync(
                    context,
                    CheatCategory.Murder,
                    $"Killed {target.PlayerId} only {now - timeline.LastKillAt:0.###}s after killing {timeline.LastKillTarget}");
            }

            return true;
        }

        protected async ValueTask<bool> ValidateMeetingTiming(CheatContext context, IClientPlayer sender)
        {
            if (Game.AntiCheat.InMeeting &&
                await sender.Client.ReportCheatAsync(
                    context,
                    CheatCategory.Meeting,
                    "Client sent a meeting while one is already in progress"))
            {
                return false;
            }

            if (Game.Options.GameMode is GameModes.HideNSeek &&
                await sender.Client.ReportCheatAsync(
                    context,
                    CheatCategory.Meeting,
                    "Client sent a meeting during Hide and Seek"))
            {
                return false;
            }

            if (!Game.AntiCheat.ShipLoaded)
            {
                return await sender.Client.ReportCheatAsync(
                    context,
                    CheatCategory.Meeting,
                    "Client sent a meeting before the map of this round was spawned");
            }

            if (Game.AntiCheat.SecondsSinceRoundStarted >= 0 &&
                Game.AntiCheat.SecondsSinceRoundStarted < 10)
            {
                return await sender.Client.ReportCheatAsync(
                    context,
                    CheatCategory.Meeting,
                    $"Client sent a meeting {Game.AntiCheat.SecondsSinceRoundStarted:0.###}s into the round (grace 10s)");
            }

            return true;
        }

        protected async ValueTask<bool> ValidateUpdateSystem(CheatContext context, IClientPlayer sender, IMessageReader reader)
        {
            if (!Game.AntiCheat.Config.EnableSabotageChecks)
            {
                return true;
            }

            async ValueTask<bool> Allowed(string message, CheatCategory category = CheatCategory.Sabotage)
                => !await sender.Client.ReportCheatAsync(context, category, message);

            var position = reader.Position;

            try
            {
                var systemType = default(SystemTypes);
                InnerPlayerControl? playerControl = null;
                var state = (byte)0;
                var malformed = reader.Position >= reader.Length;

                if (!malformed)
                {
                    systemType = (SystemTypes)reader.ReadByte();
                    playerControl = reader.ReadNetObject<InnerPlayerControl>(Game);
                    malformed = playerControl == null || reader.Position >= reader.Length;

                    if (!malformed)
                    {
                        state = reader.ReadByte();
                    }
                }

                if (malformed)
                {
                    return await Allowed("Client sent a malformed UpdateSystem RPC", CheatCategory.ProtocolExtension);
                }

                if (systemType == SystemTypes.Sabotage)
                {
                    systemType = (SystemTypes)state;
                    state = 0x80;
                }

                if (!sender.IsHost && !playerControl!.IsOwnedBy(sender))
                {
                    if (await sender.Client.ReportCheatAsync(context, CheatCategory.Ownership, $"Client sent {nameof(RpcCalls.UpdateSystem)} for a {nameof(InnerPlayerControl)} it does not own"))
                    {
                        return false;
                    }
                }

                if (!Game.AntiCheat.ShipGraceOver)
                {
                    return true;
                }

                var startingSabotage = (state & 0x80) != 0;
                var hideAndSeek = Game.Options.GameMode is GameModes.HideNSeek;

                if (hideAndSeek && systemType == SystemTypes.Doors && (state & 192) != 64)
                {
                    return await Allowed($"Client sent a door state 0x{state:X2} during Hide and Seek");
                }

                if (hideAndSeek && (startingSabotage || systemType == SystemTypes.MushroomMixupSabotage))
                {
                    return await Allowed($"Client started a sabotage of {systemType} during Hide and Seek");
                }

                if (systemType == SystemTypes.Electrical && !startingSabotage && state > 4)
                {
                    return await Allowed($"Client sent an out of range state {state} for {systemType}", CheatCategory.ProtocolExtension);
                }

                if (Game.AntiCheat.MeetingGuardArmed)
                {
                    return await Allowed($"Client sabotaged {systemType} while a meeting is in progress");
                }

                if ((int)systemType != 16 && startingSabotage)
                {
                    var timeline = Game.AntiCheat.For(ResolveActorId(playerControl!, sender));
                    var now = AntiCheatState.Now;

                    if (timeline.LastSabotageAt >= 0 &&
                        now - timeline.LastSabotageAt < 3 &&
                        timeline.LastSabotageSystem != (int)systemType)
                    {
                        var reason = $"Sabotaged {systemType} {now - timeline.LastSabotageAt:0.###}s after {timeline.LastSabotageSystem}";
                        timeline.LastSabotageAt = now;
                        timeline.LastSabotageSystem = (int)systemType;

                        if (!await Allowed(reason))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        timeline.LastSabotageAt = now;
                        timeline.LastSabotageSystem = (int)systemType;
                    }
                }

                if (ResolveActorPlayerInfo(playerControl!, sender)?.IsImpostor != true &&
                    (startingSabotage || systemType == SystemTypes.MushroomMixupSabotage))
                {
                    return await Allowed($"Non-impostor started a sabotage of {systemType} (state 0x{state:X2})");
                }

                return true;
            }
            finally
            {
                reader.Seek(position);
            }
        }

        protected async ValueTask<bool> ValidateVoteCast(CheatContext context, IClientPlayer sender)
        {
            if (!Game.AntiCheat.Config.EnableVotingChecks)
            {
                return true;
            }

            if (!Game.AntiCheat.InMeeting &&
                await sender.Client.ReportCheatAsync(context, CheatCategory.Voting, "Client cast a vote while no meeting is in progress"))
            {
                return false;
            }

            if (sender.Character?.PlayerInfo is { IsDead: true } &&
                await sender.Client.ReportCheatAsync(context, CheatCategory.Voting, "Client cast a vote while being dead"))
            {
                return false;
            }

            return true;
        }

        protected async ValueTask<bool> ValidateVoterCount(CheatContext context, IClientPlayer sender, IMessageReader reader)
        {
            var position = reader.Position;
            int voters;

            try
            {
                voters = reader.ReadPackedInt32();
            }
            catch (Exception)
            {
                return true;
            }
            finally
            {
                reader.Seek(position);
            }

            var limit = Math.Max(Game.Options.MaxPlayers, Game.PlayerCount);
            if (voters >= 0 && voters <= limit && voters <= reader.Length - position)
            {
                return true;
            }

            return await sender.Client.ReportCheatAsync(
                context,
                CheatCategory.ProtocolExtension,
                $"Client sent a VotingComplete with {voters} voters (max {limit})");
        }

        protected async ValueTask<bool> ValidateTaskCompletion(CheatContext context, IClientPlayer sender, byte playerId)
        {
            if (!Game.AntiCheat.Config.EnableRateLimits)
            {
                return true;
            }

            var config = Game.AntiCheat.Config;
            if (!Game.AntiCheat.For(playerId).CountTask(AntiCheatState.Now, 3, 2))
            {
                return true;
            }

            return await sender.Client.ReportCheatAsync(
                context,
                CheatCategory.RateLimit,
                $"Client completed more than 3 tasks in 2s");
        }

        internal static async ValueTask<bool> ValidateRpcRate(Game game, CheatContext context, IClientPlayer sender, InnerNetObject obj, bool toPlayer)
        {
            if (!game.AntiCheat.Config.EnableRateLimits)
            {
                return true;
            }

            var ownerId = obj.OwnerId >= 0 ? obj.OwnerId : sender.Client.Id;

            if (!toPlayer && ownerId != sender.Client.Id)
            {
                return true;
            }

            var playerId = game.GetClientPlayer(ownerId)?.Character?.PlayerId;
            if (playerId == null)
            {
                return true;
            }

            var limit = game.AntiCheat.Config.RpcRateLimitPerSecond;
            if (!game.AntiCheat.For(playerId.Value).CountRpc(AntiCheatState.Now, limit))
            {
                return true;
            }

            return await sender.Client.ReportCheatAsync(
                context,
                CheatCategory.RateLimit,
                $"Client sent more than {limit} RPCs in one second");
        }

        private byte ResolveActorId(InnerPlayerControl control, IClientPlayer sender)
        {
            if (control.OwnerId >= 0)
            {
                var owner = Game.GetClientPlayer(control.OwnerId);
                if (owner?.Character != null)
                {
                    return owner.Character.PlayerId;
                }
            }

            return sender.Character?.PlayerId ?? byte.MaxValue;
        }

        private InnerPlayerInfo? ResolveActorPlayerInfo(InnerPlayerControl control, IClientPlayer sender)
        {
            if (control.PlayerInfo != null)
            {
                return control.PlayerInfo;
            }

            var owner = control.OwnerId >= 0 ? Game.GetClientPlayer(control.OwnerId) : null;
            return (InnerPlayerInfo?)(owner?.Character?.PlayerInfo ?? sender.Character?.PlayerInfo);
        }

        protected async ValueTask<bool> UnregisteredCall(CheatContext context, IClientPlayer sender)
        {
            if (await sender.Client.ReportCheatAsync(context, CheatCategory.ProtocolExtension, "Client sent unregistered call"))
            {
                return false;
            }

            return true;
        }
    }
}
