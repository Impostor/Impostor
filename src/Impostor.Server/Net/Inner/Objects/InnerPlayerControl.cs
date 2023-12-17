using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Innersloth.GameOptions.RoleOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Api.Utils;
using Impostor.Server.Events.Player;
using Impostor.Server.Net.Inner.Objects.Components;
using Impostor.Server.Net.State;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerPlayerControl : InnerNetObject
    {
        private static readonly byte ColorsCount = (byte)Enum.GetValues<ColorType>().Length;

        private readonly Game _game;
        private readonly ILogger<InnerPlayerControl> _logger;
        private readonly IEventManager _eventManager;
        private readonly IDateTimeProvider _dateTimeProvider;

        public InnerPlayerControl(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerPlayerControl> logger, IServiceProvider serviceProvider, IEventManager eventManager, IDateTimeProvider dateTimeProvider) : base(customMessageManager, game)
        {
            _game = game;
            _logger = logger;
            _eventManager = eventManager;
            _dateTimeProvider = dateTimeProvider;

            Physics = ActivatorUtilities.CreateInstance<InnerPlayerPhysics>(serviceProvider, this, _eventManager, game);
            NetworkTransform = ActivatorUtilities.CreateInstance<InnerCustomNetworkTransform>(serviceProvider, this, game);

            Components.Add(this);
            Components.Add(Physics);
            Components.Add(NetworkTransform);

            PlayerId = byte.MaxValue;
        }

        public bool IsNew { get; private set; }

        public byte PlayerId { get; private set; }

        public InnerPlayerPhysics Physics { get; }

        public InnerCustomNetworkTransform NetworkTransform { get; }

        [AllowNull]
        public InnerPlayerInfo PlayerInfo { get; internal set; }

        internal Queue<string> RequestedPlayerName { get; } = new Queue<string>();

        internal Queue<ColorType> RequestedColorId { get; } = new Queue<ColorType>();

        /// <summary> Gets or sets target that was set by the last CheckMurder RPC. </summary>
        internal IInnerPlayerControl? IsMurdering { get; set; } = null;

        internal DateTimeOffset? ProtectedOn { get; set; }

        internal IInnerPlayerControl? ProtectedBy { get; set; }

        internal bool IsProtected
        {
            get
            {
                // HnS doesn't have guardian angels
                if (Game.Options is NormalGameOptions normalGameOptions && ProtectedOn != null)
                {
                    var guardianAngelOptions = (GuardianAngelRoleOptions)normalGameOptions.RoleOptions.Roles[RoleTypes.GuardianAngel].RoleOptions;
                    var protectionExpiresAt = ProtectedOn.Value.AddSeconds(guardianAngelOptions.ProtectionDurationSeconds);
                    return protectionExpiresAt >= _dateTimeProvider.UtcNow;
                }

                return false;
            }
        }

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            if (!await ValidateHost(CheatContext.Deserialize, sender))
            {
                return;
            }

            if (initialState)
            {
                IsNew = reader.ReadBoolean();
            }

            PlayerId = reader.ReadByte();
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.PlayAnimation:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc00PlayAnimation.Deserialize(reader, out var task);
                    break;
                }

                case RpcCalls.CompleteTask:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc01CompleteTask.Deserialize(reader, out var taskId);
                    await HandleCompleteTask(sender, taskId);
                    break;
                }

                case RpcCalls.SyncSettings:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    // Clients no longer handle this packet since 2022.12.8 but continue to send it
                    break;
                }

                case RpcCalls.SetInfected:
                {
                    if (!await ValidateOwnership(call, sender) || !await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc03SetInfected.Deserialize(reader, out var infectedIds);
                    await HandleSetInfected(infectedIds);
                    break;
                }

                case RpcCalls.CheckName:
                {
                    if (!await ValidateOwnership(call, sender) || !await ValidateCmd(call, sender, target))
                    {
                        return false;
                    }

                    Rpc05CheckName.Deserialize(reader, out var name);
                    return await HandleCheckName(sender, name);
                }

                case RpcCalls.SetName:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc06SetName.Deserialize(reader, out var name);
                    return await HandleSetName(sender, name);
                }

                case RpcCalls.CheckColor:
                {
                    if (!await ValidateOwnership(call, sender) || !await ValidateCmd(call, sender, target))
                    {
                        return false;
                    }

                    Rpc07CheckColor.Deserialize(reader, out var color);
                    return await HandleCheckColor(sender, color);
                }

                case RpcCalls.SetColor:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc08SetColor.Deserialize(reader, out var color);
                    return await HandleSetColor(sender, color);
                }

                case RpcCalls.SetHatStr:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc39SetHatStr.Deserialize(reader, out var hat);
                    return true;
                }

                case RpcCalls.SetSkinStr:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc40SetSkinStr.Deserialize(reader, out var skin);
                    return true;
                }

                case RpcCalls.SetVisorStr:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc42SetVisorStr.Deserialize(reader, out var visor);
                    return true;
                }

                case RpcCalls.SetNamePlateStr:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc43SetNamePlateStr.Deserialize(reader, out var namePlate);
                    return true;
                }

                case RpcCalls.SetLevel:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc38SetLevel.Deserialize(reader, out var level);
                    return true;
                }

                case RpcCalls.ReportDeadBody:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc11ReportDeadBody.Deserialize(reader, out var targetId);
                    break;
                }

                case RpcCalls.MurderPlayer:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc12MurderPlayer.Deserialize(reader, Game, out var murdered, out var result);
                    return await HandleMurderPlayer(sender, (InnerPlayerControl?)murdered, result);
                }

                case RpcCalls.SendChat:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc13SendChat.Deserialize(reader, out var message);
                    return await HandleSendChat(sender, message);
                }

                case RpcCalls.StartMeeting:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc14StartMeeting.Deserialize(reader, out var targetId);
                    await HandleStartMeeting(targetId);
                    break;
                }

                case RpcCalls.SetScanner:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc15SetScanner.Deserialize(reader, out var on, out var scannerCount);
                    break;
                }

                case RpcCalls.SendChatNote:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc16SendChatNote.Deserialize(reader, out var playerId, out var chatNoteType);
                    break;
                }

                case RpcCalls.SetPetStr:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc41SetPetStr.Deserialize(reader, out var pet);
                    return await HandleSetPet(sender, pet);
                }

                case RpcCalls.SetStartCounter:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc18SetStartCounter.Deserialize(reader, out var sequenceId, out var startCounter);
                    return await HandleSetStartCounter(sender, sequenceId, startCounter);
                }

                case RpcCalls.UsePlatform:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc32UsePlatform.Deserialize(reader);
                    break;
                }

                case RpcCalls.SendQuickChat:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    // TODO: deserialize and expose the result in an API
                    break;
                }

                case RpcCalls.SetRole:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc44SetRole.Deserialize(reader, out var role);
                    PlayerInfo.RoleType = role;

                    if (Game.GameState == GameStates.Starting && Game.Players.All(clientPlayer => clientPlayer.Character?.PlayerInfo.RoleType != null))
                    {
                        await Game.StartedAsync();
                    }

                    break;
                }

                case RpcCalls.ProtectPlayer:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc45ProtectPlayer.Deserialize(reader, Game, out var protectTarget, out _);
                    return await HandleProtectPlayer(sender, protectTarget);
                }

                case RpcCalls.Shapeshift:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    if (!await ValidateRole(call, sender, PlayerInfo, RoleTypes.Shapeshifter))
                    {
                        return false;
                    }

                    Rpc46Shapeshift.Deserialize(reader, Game, out _, out _);

                    break;
                }

                case RpcCalls.CheckMurder:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    if (!await ValidateImpostor(call, sender, PlayerInfo))
                    {
                        return false;
                    }

                    Rpc47CheckMurder.Deserialize(reader, Game, out var murdered);
                    return await HandleCheckMurder(sender, (InnerPlayerControl?)murdered);
                }

                case RpcCalls.CheckProtect:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    if (!await ValidateRole(call, sender, PlayerInfo, RoleTypes.GuardianAngel))
                    {
                        return false;
                    }

                    Rpc48CheckProtect.Deserialize(reader, Game, out _);
                    break;
                }

                case RpcCalls.CheckZipline:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc51CheckZipline.Deserialize(reader, out var fromTop);
                    break;
                }

                case RpcCalls.UseZipline:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc52UseZipline.Deserialize(reader, out var fromTop);
                    break;
                }

                case RpcCalls.TriggerSpores:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc53TriggerSpores.Deserialize(reader, out var mushroomId);
                    break;
                }

                case RpcCalls.CheckSpore:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc54CheckSpore.Deserialize(reader, out var mushroomId);
                    break;
                }

                case RpcCalls.CheckShapeshift:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc46Shapeshift.Deserialize(reader, Game, out var playerControl, out var shouldAnimate);
                    break;
                }

                case RpcCalls.RejectShapeshift:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc56RejectShapeshift.Deserialize(reader);
                    break;
                }

                default:
                    return await base.HandleRpcAsync(sender, target, call, reader);
            }

            return true;
        }

        internal void Die(DeathReason reason)
        {
            PlayerInfo.IsDead = true;
            PlayerInfo.LastDeathReason = reason;
        }

        internal void Protect(InnerPlayerControl guardianAngel)
        {
            // NOTE: Vanilla dispells all GA shields when a kill is blocked, so it suffices to keep track of the last protection action
            ProtectedOn = _dateTimeProvider.UtcNow;
            ProtectedBy = guardianAngel;
        }

        private async ValueTask HandleCompleteTask(ClientPlayer sender, uint taskId)
        {
            var task = PlayerInfo.Tasks.ElementAtOrDefault((int)taskId);

            if (task != null)
            {
                task.Complete = true;
                await _eventManager.CallAsync(new PlayerCompletedTaskEvent(Game, sender, this, task));
            }
            else
            {
                _logger.LogWarning($"Client sent {nameof(RpcCalls.CompleteTask)} with a taskIndex that is not in their {nameof(InnerPlayerInfo)}");
            }
        }

        private async ValueTask HandleSetInfected(ReadOnlyMemory<byte> infectedIds)
        {
            for (var i = 0; i < infectedIds.Length; i++)
            {
                var player = Game.GameNet.GameData!.GetPlayerById(infectedIds.Span[i]);
                if (player != null)
                {
                    // player.IsImpostor = true;
                }
            }

            if (Game.GameState == GameStates.Starting)
            {
                await Game.StartedAsync();
            }
        }

        private async ValueTask<bool> HandleCheckName(ClientPlayer sender, string name)
        {
            if (name.Length > 10)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.CheckName, "Client sent name exceeding 10 characters"))
                {
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(name) || !name.All(TextBox.IsCharAllowed))
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.CheckName, "Client sent name containing illegal characters"))
                {
                    return false;
                }
            }

            if (sender.Client.Name != name)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.CheckName, "Client sent name not matching his name from handshake"))
                {
                    return false;
                }
            }

            RequestedPlayerName.Enqueue(name);

            return true;
        }

        private async ValueTask<bool> HandleSetName(ClientPlayer sender, string name)
        {
            if (Game.GameState == GameStates.Started)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.SetColor, "Client tried to set a name midgame"))
                {
                    return false;
                }
            }

            if (sender.IsOwner(this))
            {
                if (Game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.PlayerName == name))
                {
                    if (await sender.Client.ReportCheatAsync(RpcCalls.SetName, "Client sent name that is already used"))
                    {
                        return false;
                    }
                }

                if (sender.Client.Name != name)
                {
                    if (await sender.Client.ReportCheatAsync(RpcCalls.SetName, "Client sent name not matching his name from handshake"))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (RequestedPlayerName.Any())
                {
                    var expected = RequestedPlayerName.Dequeue();

                    if (Game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.PlayerName == expected))
                    {
                        var i = 1;
                        while (true)
                        {
                            var text = expected + " " + i;

                            if (Game.Players.All(x => x.Character == null || x.Character == this || x.Character.PlayerInfo.PlayerName != text))
                            {
                                expected = text;
                                break;
                            }

                            i++;
                        }
                    }

                    if (name != expected)
                    {
                        _logger.LogWarning($"Client sent {nameof(RpcCalls.SetName)} with incorrect name");
                        await SetNameAsync(expected);
                        return false;
                    }
                }
                else
                {
                    if (await sender.Client.ReportCheatAsync(RpcCalls.SetName, $"Client sent {nameof(RpcCalls.SetName)} for a player that didn't request it"))
                    {
                        return false;
                    }
                }
            }

            PlayerInfo.PlayerName = name;

            return true;
        }

        private async ValueTask<bool> HandleCheckColor(ClientPlayer sender, ColorType color)
        {
            if ((byte)color > ColorsCount)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.CheckColor, "Client sent invalid color"))
                {
                    return false;
                }
            }

            RequestedColorId.Enqueue(color);

            return true;
        }

        private async ValueTask<bool> HandleSetColor(ClientPlayer sender, ColorType color)
        {
            if (Game.GameState == GameStates.Started)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.SetColor, "Client tried to set a color midgame"))
                {
                    return false;
                }
            }

            if (sender.IsOwner(this))
            {
                if (Game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.CurrentOutfit.Color == color))
                {
                    if (await sender.Client.ReportCheatAsync(RpcCalls.SetColor, "Client sent a color that is already used"))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!RequestedColorId.Any())
                {
                    _logger.LogWarning($"Client sent {nameof(RpcCalls.SetColor)} for a player that didn't request it");
                    return false;
                }

                var expected = RequestedColorId.Dequeue();

                while (Game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.CurrentOutfit.Color == expected))
                {
                    expected = (ColorType)(((byte)expected + 1) % ColorsCount);
                }

                if (color != expected)
                {
                    _logger.LogWarning($"Client sent {nameof(RpcCalls.SetColor)} with incorrect color");
                    await SetColorAsync(expected);
                    return false;
                }
            }

            PlayerInfo.CurrentOutfit.Color = color;

            return true;
        }

        private async ValueTask<bool> HandleSetHat(ClientPlayer sender, string hat)
        {
            if (Game.GameState == GameStates.Started && await sender.Client.ReportCheatAsync(RpcCalls.SetHat, "Client tried to change hat while not in lobby"))
            {
                return false;
            }

            PlayerInfo.CurrentOutfit.HatId = hat;

            return true;
        }

        private async ValueTask<bool> HandleSetSkin(ClientPlayer sender, string skin)
        {
            if (Game.GameState == GameStates.Started && await sender.Client.ReportCheatAsync(RpcCalls.SetSkin, "Client tried to change skin while not in lobby"))
            {
                return false;
            }

            PlayerInfo.CurrentOutfit.SkinId = skin;

            return true;
        }

        private async ValueTask<bool> HandleCheckMurder(ClientPlayer sender, InnerPlayerControl? target)
        {
            if (!PlayerInfo.CanMurder(Game, _dateTimeProvider))
            {
                if (IsMurdering == target)
                {
                    // This request was made too quickly by spamming the kill button, cancel it if we're in server authoritive mode
                    return _game.IsHostAuthoritive;
                }
                else if (await sender.Client.ReportCheatAsync(RpcCalls.CheckMurder, "Client tried to murder too fast"))
                {
                    return false;
                }
            }

            if (target == null || target.PlayerInfo.IsImpostor)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.CheckMurder, "Client tried to murder invalid target"))
                {
                    return false;
                }
            }

            PlayerInfo.LastMurder = _dateTimeProvider.UtcNow - TimeSpan.FromMilliseconds(sender.Client.Connection.AveragePing);
            IsMurdering = target;

            // Check if host authority mode is on
            if (_game.IsHostAuthoritive)
            {
                // Pass the RPC on unharmed, the client will handle it
                return true;
            }

            if (target != null)
            {
                var result = target.IsProtected ? MurderResultFlags.FailedProtected : MurderResultFlags.Succeeded;

                var evt = new PlayerCheckMurderEvent(Game, sender, this, target, result);
                await _eventManager.CallAsync(evt);

                if (!evt.IsCancelled)
                {
                    target.ProtectedOn = null; // Clear GA protection in all cases
                    await MurderPlayerAsync(target, evt.Result);
                }
            }

            return false;
        }

        private async ValueTask<bool> HandleMurderPlayer(ClientPlayer sender, InnerPlayerControl? target, MurderResultFlags result)
        {
            if (!_game.IsHostAuthoritive)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.MurderPlayer, "Client tried to murder directly"))
                {
                    return false;
                }
            }

            if (target == null || target.PlayerInfo.IsImpostor)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.MurderPlayer, "Client tried to murder invalid target"))
                {
                    return false;
                }
            }

            // If the host is also the impostor that committed the murder, CheckMurder is actually sent *after* the MurderPlayer RPC
            if (sender.Character != this && target != IsMurdering)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.MurderPlayer, "Host tried to murder incorrect target"))
                {
                    return false;
                }
            }

            if (target != null && !target.PlayerInfo.IsDead)
            {
                // In host authoritive mode every client has to figure out if the kill was prevented by guardian protection on it's own
                if ((result & MurderResultFlags.Succeeded) != 0 && target.IsProtected)
                {
                    result = (result & ~MurderResultFlags.Succeeded) | MurderResultFlags.FailedProtected;
                }

                if (!result.IsFailed())
                {
                    target.Die(DeathReason.Kill);
                }
                else if ((result & MurderResultFlags.FailedProtected) != 0)
                {
                    target.ProtectedOn = null;
                }

                await _eventManager.CallAsync(new PlayerMurderEvent(Game, sender, this, target, result));
            }

            IsMurdering = null;

            return true;
        }

        private async ValueTask<bool> HandleProtectPlayer(ClientPlayer sender, IInnerPlayerControl? target)
        {
            if (target == null)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.CheckProtect, "Client tried to protect invalid target"))
                {
                    return false;
                }

                return true;
            }

            if (PlayerInfo.RoleType != RoleTypes.GuardianAngel)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.CheckProtect, "Client tried to protect but it wasn't a guardian angel"))
                {
                    return false;
                }
            }

            ((InnerPlayerControl)target).Protect(this);

            return true;
        }

        private async ValueTask<bool> HandleSendChat(ClientPlayer sender, string message)
        {
            var @event = new PlayerChatEvent(Game, sender, this, message);
            await _eventManager.CallAsync(@event);

            return !@event.IsCancelled;
        }

        private async ValueTask HandleStartMeeting(byte targetId)
        {
            var deadPlayer = Game.GameNet.GameData!.GetPlayerById(targetId)?.Controller;
            await _eventManager.CallAsync(new PlayerStartMeetingEvent(Game, Game.GetClientPlayer(this.OwnerId)!, this, deadPlayer));
        }

        private async ValueTask<bool> HandleSetPet(ClientPlayer sender, string pet)
        {
            if (Game.GameState == GameStates.Started && await sender.Client.ReportCheatAsync(RpcCalls.SetPet, "Client tried to change pet while not in lobby"))
            {
                return false;
            }

            PlayerInfo.CurrentOutfit.PetId = pet;

            return true;
        }

        private async ValueTask<bool> HandleSetStartCounter(ClientPlayer sender, int sequenceId, sbyte startCounter)
        {
            if (!sender.IsHost && startCounter != -1)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.MurderPlayer, "Client tried to set start counter as a non-host"))
                {
                    return false;
                }
            }

            if (startCounter != -1)
            {
                await _eventManager.CallAsync(new PlayerSetStartCounterEvent(Game, sender, this, (byte)startCounter));
            }

            return true;
        }
    }
}
