using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.Customization;
using Impostor.Api.Net;
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
        private readonly ILogger<InnerPlayerControl> _logger;
        private readonly IEventManager _eventManager;
        private readonly Game _game;

        private static Dictionary<RpcCalls, RpcInfo> Rpcs { get; } = new Dictionary<RpcCalls, RpcInfo>
        {
            [RpcCalls.PlayAnimation] = new RpcInfo(),
            [RpcCalls.CompleteTask] = new RpcInfo(),
            [RpcCalls.SyncSettings] = new RpcInfo
            {
                CheckOwnership = false, RequireHost = true,
            },
            [RpcCalls.SetInfected] = new RpcInfo
            {
                RequireHost = true,
            },
            [RpcCalls.CheckName] = new RpcInfo
            {
                TargetType = RpcTargetType.Cmd,
            },
            [RpcCalls.SetName] = new RpcInfo
            {
                CheckOwnership = false, RequireHost = true,
            },
            [RpcCalls.CheckColor] = new RpcInfo
            {
                TargetType = RpcTargetType.Cmd,
            },
            [RpcCalls.SetColor] = new RpcInfo
            {
                CheckOwnership = false, RequireHost = true,
            },
            [RpcCalls.SetHat] = new RpcInfo(),
            [RpcCalls.SetSkin] = new RpcInfo(),
            [RpcCalls.CompleteTask] = new RpcInfo(),
            [RpcCalls.ReportDeadBody] = new RpcInfo(),
            [RpcCalls.MurderPlayer] = new RpcInfo(),
            [RpcCalls.SendChat] = new RpcInfo(),
            [RpcCalls.StartMeeting] = new RpcInfo
            {
                CheckOwnership = false, RequireHost = true,
            },
            [RpcCalls.SetScanner] = new RpcInfo(),
            [RpcCalls.SendChatNote] = new RpcInfo(),
            [RpcCalls.SetPet] = new RpcInfo(),
            [RpcCalls.SetStartCounter] = new RpcInfo(),
        };

        public InnerPlayerControl(ILogger<InnerPlayerControl> logger, IServiceProvider serviceProvider, IEventManager eventManager, Game game)
        {
            _logger = logger;
            _eventManager = eventManager;
            _game = game;

            Physics = ActivatorUtilities.CreateInstance<InnerPlayerPhysics>(serviceProvider, this, _eventManager, _game);
            NetworkTransform = ActivatorUtilities.CreateInstance<InnerCustomNetworkTransform>(serviceProvider, this, _game);

            Components.Add(this);
            Components.Add(Physics);
            Components.Add(NetworkTransform);

            PlayerId = byte.MaxValue;
        }

        public bool IsNew { get; private set; }

        public byte PlayerId { get; private set; }

        public InnerPlayerPhysics Physics { get; }

        public InnerCustomNetworkTransform NetworkTransform { get; }

        public InnerPlayerInfo PlayerInfo { get; internal set; }

        internal Queue<string> RequestedPlayerName { get; } = new Queue<string>();

        internal Queue<ColorType> RequestedColorId { get; } = new Queue<ColorType>();

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            if (!sender.IsHost)
            {
                if (await sender.Client.ReportCheatAsync(CheatContext.Deserialize, $"Client attempted to send data for {nameof(InnerPlayerControl)} as non-host"))
                {
                    return;
                }
            }

            if (initialState)
            {
                IsNew = reader.ReadBoolean();
            }

            PlayerId = reader.ReadByte();
        }

        internal void Die(DeathReason reason)
        {
            PlayerInfo.IsDead = true;
            PlayerInfo.LastDeathReason = reason;
        }

        public override async ValueTask<bool> HandleRpc(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            if (!await TestRpc(sender, target, call, Rpcs))
            {
                return false;
            }

            switch (call)
            {
                case RpcCalls.PlayAnimation:
                {
                    Rpc00PlayAnimation.Deserialize(reader, out var task);
                    break;
                }

                case RpcCalls.CompleteTask:
                {
                    Rpc01CompleteTask.Deserialize(reader, out var taskId);
                    await HandleCompleteTask(sender, taskId);
                    break;
                }

                case RpcCalls.SyncSettings:
                {
                    Rpc02SyncSettings.Deserialize(reader, _game.Options);
                    break;
                }

                case RpcCalls.SetInfected:
                {
                    Rpc03SetInfected.Deserialize(reader, out var infectedIds);
                    await HandleSetInfected(infectedIds);
                    break;
                }

                case RpcCalls.CheckName:
                {
                    Rpc05CheckName.Deserialize(reader, out var name);
                    return await HandleCheckName(sender, name);
                }

                case RpcCalls.SetName:
                {
                    Rpc06SetName.Deserialize(reader, out var name);
                    return await HandleSetName(sender, name);
                }

                case RpcCalls.CheckColor:
                {
                    Rpc07CheckColor.Deserialize(reader, out var color);
                    return await HandleCheckColor(sender, color);
                }

                case RpcCalls.SetColor:
                {
                    Rpc08SetColor.Deserialize(reader, out var color);
                    return await HandleSetColor(sender, color);
                }

                case RpcCalls.SetHat:
                {
                    Rpc09SetHat.Deserialize(reader, out var hat);
                    return await HandleSetHat(sender, hat);
                }

                case RpcCalls.SetSkin:
                {
                    Rpc10SetSkin.Deserialize(reader, out var skin);
                    return await HandleSetSkin(sender, skin);
                }

                case RpcCalls.ReportDeadBody:
                {
                    Rpc11ReportDeadBody.Deserialize(reader, out var targetId);
                    break;
                }

                case RpcCalls.MurderPlayer:
                {
                    Rpc12MurderPlayer.Deserialize(reader, _game, out var murdered);
                    return await HandleMurderPlayer(sender, murdered);
                }

                case RpcCalls.SendChat:
                {
                    Rpc13SendChat.Deserialize(reader, out var message);
                    return await HandleSendChat(sender, message);
                }

                case RpcCalls.StartMeeting:
                {
                    Rpc14StartMeeting.Deserialize(reader, out var targetId);
                    await HandleStartMeeting(targetId);
                    break;
                }

                case RpcCalls.SetScanner:
                {
                    Rpc15SetScanner.Deserialize(reader, out var on, out var scannerCount);
                    break;
                }

                case RpcCalls.SendChatNote:
                {
                    Rpc16SendChatNote.Deserialize(reader, out var playerId, out var chatNoteType);
                    break;
                }

                case RpcCalls.SetPet:
                {
                    Rpc17SetPet.Deserialize(reader, out var pet);
                    return await HandleSetPet(sender, pet);
                }

                case RpcCalls.SetStartCounter:
                {
                    Rpc18SetStartCounter.Deserialize(reader, out var sequenceId, out var startCounter);
                    return await HandleSetStartCounter(sender, sequenceId, startCounter);
                }

                case RpcCalls.CustomRpc:
                {
                    await HandleCustomRpc(reader, _game);
                    break;
                }
            }

            return true;
        }

        private async ValueTask HandleCompleteTask(ClientPlayer sender, uint taskId)
        {
            var task = PlayerInfo.Tasks.ElementAtOrDefault((int)taskId);

            if (task != null)
            {
                task.Complete = true;
                await _eventManager.CallAsync(new PlayerCompletedTaskEvent(_game, sender, this, task));
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
                var player = _game.GameNet.GameData.GetPlayerById(infectedIds.Span[i]);
                if (player != null)
                {
                    player.IsImpostor = true;
                }
            }

            if (_game.GameState == GameStates.Starting)
            {
                await _game.StartedAsync();
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
            if (_game.GameState != GameStates.NotStarted)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.SetColor, "Client tried to set a name midgame"))
                {
                    return false;
                }
            }

            if (sender.IsOwner(this))
            {
                if (_game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.PlayerName == name))
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

                if (_game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.PlayerName == expected))
                {
                    var i = 1;
                    while (true)
                    {
                        string text = expected + " " + i;

                        if (_game.Players.All(x => x.Character == null || x.Character == this || x.Character.PlayerInfo.PlayerName != text))
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
            if (color > (ColorType)Enum.GetValues<ColorType>().Length)
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
            if (_game.GameState != GameStates.NotStarted)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.SetColor, "Client tried to set a color midgame"))
                {
                    return false;
                }
            }

            if (sender.IsOwner(this))
            {
                if (_game.Players.Any(x => x.Character != null && x.Character != this && x.Character.PlayerInfo.ColorId == (byte)color))
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

                while (_game.Players.Any(x => x.Character != null && x.Character != this && (ColorType)x.Character.PlayerInfo.ColorId == expected))
                {
                    expected = (ColorType)(((byte)expected + 1) % Enum.GetValues<ColorType>().Length);
                }

                if (color != expected)
                {
                    _logger.LogWarning($"Client sent {nameof(RpcCalls.SetColor)} with incorrect color");
                    await SetColorAsync(expected);
                    return false;
                }
            }

            PlayerInfo.ColorId = (byte)color;

            return true;
        }

        private async ValueTask<bool> HandleSetHat(ClientPlayer sender, HatType hat)
        {
            if (_game.GameState != GameStates.NotStarted && await sender.Client.ReportCheatAsync(RpcCalls.SetHat, "Client tried to change hat while not in lobby"))
            {
                return false;
            }

            PlayerInfo.HatId = (byte)hat;

            return true;
        }

        private async ValueTask<bool> HandleSetSkin(ClientPlayer sender, SkinType skin)
        {
            if (_game.GameState != GameStates.NotStarted && await sender.Client.ReportCheatAsync(RpcCalls.SetSkin, "Client tried to change skin while not in lobby"))
            {
                return false;
            }

            PlayerInfo.SkinId = (byte)skin;

            return true;
        }

        private async ValueTask<bool> HandleMurderPlayer(ClientPlayer sender, IInnerPlayerControl? target)
        {
            if (!PlayerInfo.IsImpostor)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.MurderPlayer, "Client tried to murder as a crewmate"))
                {
                    return false;
                }
            }

            if (!PlayerInfo.CanMurder(_game))
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

            PlayerInfo.LastMurder = DateTimeOffset.UtcNow;

            if (!target.PlayerInfo.IsDead)
            {
                ((InnerPlayerControl)target).Die(DeathReason.Kill);
                await _eventManager.CallAsync(new PlayerMurderEvent(_game, sender, this, target));
            }

            return true;
        }

        private async ValueTask<bool> HandleSendChat(ClientPlayer sender, string message)
        {
            var @event = new PlayerChatEvent(_game, sender, this, message);
            await _eventManager.CallAsync(@event);

            return !@event.IsCancelled;
        }

        private async ValueTask HandleStartMeeting(byte targetId)
        {
            var deadPlayer = _game.GameNet.GameData.GetPlayerById(targetId)?.Controller;
            await _eventManager.CallAsync(new PlayerStartMeetingEvent(_game, _game.GetClientPlayer(this.OwnerId), this, deadPlayer));
        }

        private async ValueTask<bool> HandleSetPet(ClientPlayer sender, PetType pet)
        {
            if (_game.GameState != GameStates.NotStarted && await sender.Client.ReportCheatAsync(RpcCalls.SetHat, "Client tried to change pet while not in lobby"))
            {
                return false;
            }

            PlayerInfo.PetId = (byte)pet;

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
                await _eventManager.CallAsync(new PlayerSetStartCounterEvent(_game, sender, this, (byte)startCounter));
            }

            return true;
        }
    }
}
