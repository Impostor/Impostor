using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Rpcs;
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

        private readonly ILogger<InnerPlayerControl> _logger;
        private readonly IEventManager _eventManager;
        private readonly IDateTimeProvider _dateTimeProvider;

        public InnerPlayerControl(Game game, ILogger<InnerPlayerControl> logger, IServiceProvider serviceProvider, IEventManager eventManager, IDateTimeProvider dateTimeProvider) : base(game)
        {
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

                    Rpc02SyncSettings.Deserialize(reader, Game.Options);
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

                case RpcCalls.SetHat:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc09SetHat.Deserialize(reader, out var hat);
                    return await HandleSetHat(sender, hat);
                }

                case RpcCalls.SetSkin:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc10SetSkin.Deserialize(reader, out var skin);
                    return await HandleSetSkin(sender, skin);
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
                    if (!await ValidateOwnership(call, sender) || !await ValidateImpostor(call, sender, PlayerInfo))
                    {
                        return false;
                    }

                    Rpc12MurderPlayer.Deserialize(reader, Game, out var murdered);
                    return await HandleMurderPlayer(sender, murdered);
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

                case RpcCalls.SetPet:
                {
                    if (!await ValidateOwnership(call, sender))
                    {
                        return false;
                    }

                    Rpc17SetPet.Deserialize(reader, out var pet);
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

                default:
                    return await UnregisteredCall(call, sender);
            }

            return true;
        }

        internal void Die(DeathReason reason)
        {
            PlayerInfo.IsDead = true;
            PlayerInfo.LastDeathReason = reason;
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
                    player.IsImpostor = true;
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
                if (!RequestedPlayerName.Any())
                {
                    _logger.LogWarning($"Client sent {nameof(RpcCalls.SetName)} for a player that didn't request it");
                    return false;
                }

                var expected = RequestedPlayerName.Dequeue();

                if (Game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.PlayerName == expected))
                {
                    var i = 1;
                    while (true)
                    {
                        string text = expected + " " + i;

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
                if (Game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.Color == color))
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

                while (Game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.Color == expected))
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

            PlayerInfo.Color = color;

            return true;
        }

        private async ValueTask<bool> HandleSetHat(ClientPlayer sender, HatType hat)
        {
            if (Game.GameState == GameStates.Started && await sender.Client.ReportCheatAsync(RpcCalls.SetHat, "Client tried to change hat while not in lobby"))
            {
                return false;
            }

            PlayerInfo.Hat = hat;

            return true;
        }

        private async ValueTask<bool> HandleSetSkin(ClientPlayer sender, SkinType skin)
        {
            if (Game.GameState == GameStates.Started && await sender.Client.ReportCheatAsync(RpcCalls.SetSkin, "Client tried to change skin while not in lobby"))
            {
                return false;
            }

            PlayerInfo.Skin = skin;

            return true;
        }

        private async ValueTask<bool> HandleMurderPlayer(ClientPlayer sender, IInnerPlayerControl? target)
        {
            if (!PlayerInfo.CanMurder(Game, _dateTimeProvider))
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.MurderPlayer, "Client tried to murder too fast"))
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

            PlayerInfo.LastMurder = _dateTimeProvider.UtcNow;

            if (target != null && !target.PlayerInfo.IsDead)
            {
                ((InnerPlayerControl)target).Die(DeathReason.Kill);
                await _eventManager.CallAsync(new PlayerMurderEvent(Game, sender, this, target));
            }

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

        private async ValueTask<bool> HandleSetPet(ClientPlayer sender, PetType pet)
        {
            if (Game.GameState == GameStates.Started && await sender.Client.ReportCheatAsync(RpcCalls.SetPet, "Client tried to change pet while not in lobby"))
            {
                return false;
            }

            PlayerInfo.Pet = pet;

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
