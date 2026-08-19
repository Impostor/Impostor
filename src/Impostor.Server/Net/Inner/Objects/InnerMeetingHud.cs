using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Events.Player;
using Impostor.Api.Innersloth;
using Impostor.Api.Innersloth.GameOptions;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Events.Meeting;
using Impostor.Server.Events.Player;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerMeetingHud : InnerNetObject
    {
        private static readonly GameVersion JudgeMinVersion = new GameVersion(2026, 7, 15); // 18.0

        private readonly ILogger<InnerMeetingHud> _logger;
        private readonly IEventManager _eventManager;
        private readonly CancellationTokenSource _timerToken;
        private readonly List<JudgeOverrule> _judgeOverrules = new();

        [AllowNull]
        private PlayerVoteArea[] _playerStates;

        public InnerMeetingHud(ICustomMessageManager<ICustomRpc> customMessageManager, Game game, ILogger<InnerMeetingHud> logger, IEventManager eventManager) : base(customMessageManager, game)
        {
            _logger = logger;
            _eventManager = eventManager;
            _playerStates = null;

            Components.Add(this);

            _timerToken = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                try
                {
                    const float AnimationTime = 0.25f + 0.5f + 0.4f + 3f + 0.75f + 5f;
                    if (Game.Options.GameMode is GameModes.Normal or GameModes.NormalFools)
                    {
                        var options = (NormalGameOptions)Game.Options;
                        await Task.Delay(TimeSpan.FromSeconds(AnimationTime + options.DiscussionTime + options.VotingTime), _timerToken.Token);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                await HandleVotingCompleteAsync();
            });
        }

        public InnerPlayerInfo? Reporter { get; private set; }

        public IReadOnlyCollection<IInnerMeetingHud.IJudgeOverrule> JudgeOverrules => _judgeOverrules;

        public override ValueTask<bool> SerializeAsync(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override async ValueTask DeserializeAsync(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
            if (!await ValidateHost(CheatContext.Deserialize, sender) || !await ValidateBroadcast(CheatContext.Deserialize, sender, target))
            {
                return;
            }

            if (initialState)
            {
                PopulateButtons();
            }

            var length = reader.ReadPackedUInt32();

            for (var i = 0; i < length; i++)
            {
                var inner = reader.ReadMessage();
                var playerVoteArea = _playerStates.SingleOrDefault(x => x.TargetPlayer.PlayerId == inner.Tag);

                if (playerVoteArea != null)
                {
                    var clientPlayer = Game.Players.SingleOrDefault(x => x.Character?.PlayerId == playerVoteArea.TargetPlayer.PlayerId);
                    var updateVote = !playerVoteArea.DidVote && (clientPlayer?.IsHost ?? false) && playerVoteArea.VoteType != VoteType.Missed;

                    playerVoteArea.Deserialize(inner, updateVote);

                    if (updateVote)
                    {
                        await HandleVoteAsync(playerVoteArea);
                        await CheckForEndVotingAsync();
                    }

                    if (initialState && playerVoteArea.DidReport)
                    {
                        Reporter = playerVoteArea.TargetPlayer;
                    }
                }
            }

            if (sender.Client.GameVersion >= JudgeMinVersion)
            {
                var overruleQueueListLength = reader.ReadPackedInt32();
                if (overruleQueueListLength > 0)
                {
                    _judgeOverrules.Clear();
                    for (var i = 0; i < overruleQueueListLength; i++)
                    {
                        _judgeOverrules.Add(JudgeOverrule.Deserialize(reader));
                    }
                }
            }
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.CloseMeeting:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc22Close.Deserialize(reader);
                    _judgeOverrules.Clear();
                    break;
                }

                case RpcCalls.VotingComplete:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    var hasOverruleFields = sender.Client.GameVersion >= JudgeMinVersion;
                    Rpc23VotingComplete.Deserialize(reader, hasOverruleFields, out var states, out var playerId, out var tie, out var wasOverruled, out var overrideId);
                    foreach (var messageReader in states)
                    {
                        messageReader.Dispose();
                    }

                    // This would be a nice place to implement an anti cheat.
                    // But for whatever reason host sends VotingComplete before sending his vote.
                    // Also every client executes his own VotingComplete after other client players CastVote rpc, like wtf.
                    break;
                }

                case RpcCalls.CastVote:
                {
                    Rpc24CastVote.Deserialize(reader, out var playerId, out var suspectPlayerId);
                    return await HandleCastVoteAsync(sender, target, playerId, suspectPlayerId);
                }

                case RpcCalls.ClearVote:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc25ClearVote.Deserialize(reader);
                    break;
                }

                case RpcCalls.QueueOverruleVotes:
                {
                    Rpc66QueueOverruleVotes.Deserialize(reader, out var judgePlayerId, out var targetPlayerId, out var overruleNonce);
                    return await HandleQueueOverruleVotesAsync(sender, target, judgePlayerId, targetPlayerId, overruleNonce);
                }

                default:
                    return await base.HandleRpcAsync(sender, target, call, reader);
            }

            return true;
        }

        private void PopulateButtons()
        {
            _playerStates = Game.GameNet.GameData!.Players.Values
                .OrderBy(x => x.Controller?.NetId) // The host player hold MeetingHud players list sorted by NetId
                .Select(x => new PlayerVoteArea(this, x, x.Disconnected || x.IsDead))
                .ToArray();
        }

        private async ValueTask HandleVoteAsync(PlayerVoteArea playerState)
        {
            if (playerState.DidVote && !playerState.IsDead)
            {
                var player = playerState.TargetPlayer.Controller!;
                await _eventManager.CallAsync(new PlayerVotedEvent(Game, Game.GetClientPlayer(player!.OwnerId)!, player, playerState.VoteType!.Value, playerState.VotedFor));
            }
        }

        private async ValueTask<bool> HandleCastVoteAsync(ClientPlayer sender, ClientPlayer? target, byte playerId, byte suspectPlayerId)
        {
            if (sender.IsHost)
            {
                if (!await ValidateBroadcast(RpcCalls.CastVote, sender, target))
                {
                    return false;
                }
            }
            else
            {
                if (!await ValidateCmd(RpcCalls.CastVote, sender, target))
                {
                    return false;
                }
            }

            if (playerId != sender.Character!.PlayerId)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.CastVote, CheatCategory.Ownership, $"Client sent {nameof(RpcCalls.CastVote)} to an unowned {nameof(InnerPlayerControl)}"))
                {
                    return false;
                }
            }

            if (!sender.IsHost)
            {
                var playerVoteArea = _playerStates.Single(x => x.TargetPlayer.PlayerId == playerId);
                playerVoteArea.SetVotedFor(suspectPlayerId);
                await HandleVoteAsync(playerVoteArea);
                await CheckForEndVotingAsync();
            }

            return true;
        }

        private async ValueTask<bool> HandleQueueOverruleVotesAsync(ClientPlayer sender, ClientPlayer? target, byte judgePlayerId, byte targetPlayerId, ushort overruleNonce)
        {
            if (!await ValidateRole(RpcCalls.QueueOverruleVotes, sender, sender.Character?.PlayerInfo, RoleTypes.Judge) ||
                !await ValidateTarget(RpcCalls.QueueOverruleVotes, sender, target) ||
                !await ValidateHost(RpcCalls.QueueOverruleVotes, target!))
            {
                return false;
            }

            // A Judge can only queue an overrule for themself.
            if (judgePlayerId != sender.Character!.PlayerId)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.QueueOverruleVotes, CheatCategory.Ownership, "Client sent a Judge overrule for another player"))
                {
                    return false;
                }
            }

            // The client only generates nonces in the 1..65535 range.
            if (overruleNonce == 0)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.QueueOverruleVotes, CheatCategory.GameFlow, "Client sent a Judge overrule with an uninitialized nonce"))
                {
                    return false;
                }
            }

            // A Judge can only overrule once per meeting.
            if (_judgeOverrules.Any(overrule => overrule.JudgePlayerId == judgePlayerId))
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.QueueOverruleVotes, CheatCategory.GameFlow, "Client sent more than one Judge overrule in the same meeting"))
                {
                    return false;
                }
            }

            // The overruled player must exist.
            if (Game.GameNet.GameData!.GetPlayerById(targetPlayerId) == null)
            {
                if (await sender.Client.ReportCheatAsync(RpcCalls.QueueOverruleVotes, CheatCategory.InvalidObject, "Client sent a Judge overrule for an unknown player"))
                {
                    return false;
                }
            }

            AddOrUpdateJudgeOverrule(judgePlayerId, targetPlayerId, overruleNonce);

            return true;
        }

        private void AddOrUpdateJudgeOverrule(byte judgePlayerId, byte targetPlayerId, ushort overruleNonce)
        {
            var existing = _judgeOverrules.Find(overrule => overrule.JudgePlayerId == judgePlayerId);
            if (existing != null)
            {
                existing.OverruledPlayerId = targetPlayerId;
                existing.OverruleNonce = overruleNonce;
            }
            else
            {
                _judgeOverrules.Add(new JudgeOverrule(judgePlayerId, targetPlayerId, overruleNonce));
            }
        }
        
        private JudgeOverrule? GetWinningOverrule()
        {
            var gameData = Game.GameNet.GameData!;
            foreach (var overrule in _judgeOverrules)
            {
                var judge = gameData.GetPlayerById(overrule.JudgePlayerId);
                var target = gameData.GetPlayerById(overrule.OverruledPlayerId);

                if (judge != null && target != null && !judge.Disconnected && !target.Disconnected)
                {
                    return overrule;
                }
            }

            return null;
        }

        private async ValueTask CheckForEndVotingAsync()
        {
            if (_playerStates.All(ps => ps.IsDead || ps.DidVote))
            {
                await HandleVotingCompleteAsync();
            }
        }

        private KeyValuePair<byte, int> MaxPair(Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            foreach (var keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                {
                    tie = true;
                }
            }

            return result;
        }

        private Dictionary<byte, int> CalculateVotes()
        {
            var players = new Dictionary<byte, int>();
            foreach (var playerVoteArea in _playerStates)
            {
                if (!playerVoteArea.IsDead && playerVoteArea.DidVote && playerVoteArea.VoteType != VoteType.Missed)
                {
                    if (players.TryGetValue(playerVoteArea.VotedForId, out var current))
                    {
                        players[playerVoteArea.VotedForId] = current + 1;
                    }
                    else
                    {
                        players[playerVoteArea.VotedForId] = 1;
                    }
                }
            }

            return players;
        }

        private async ValueTask HandleVotingCompleteAsync()
        {
            _timerToken.Cancel();

            foreach (var playerVoteArea in _playerStates)
            {
                if (!playerVoteArea.DidVote)
                {
                    playerVoteArea.SetVotedFor((byte)VoteType.Missed);
                    await HandleVoteAsync(playerVoteArea);
                }
            }

            var self = this.CalculateVotes();
            var max = MaxPair(self, out var tie);
            var exiled = tie ? null : Game.GameNet.GameData!.GetPlayerById(max.Key)?.Controller;
            
            // exile or others?
            var wasOverruled = false;
            ushort overrideId = 0;

            var winningOverrule = GetWinningOverrule();
            if (winningOverrule != null)
            {
                wasOverruled = true;
                overrideId = winningOverrule.OverruleNonce;

                var overruled = Game.GameNet.GameData!.GetPlayerById(winningOverrule.OverruledPlayerId);
                if (overruled != null && overruled.IsImpostor)
                {
                    exiled = overruled.Controller;
                }
                else
                {
                    var judge = Game.GameNet.GameData!.GetPlayerById(winningOverrule.JudgePlayerId);
                    exiled = judge?.Controller;
                }
            }

            if (exiled != null && exiled.PlayerInfo != null)
            {
                exiled.PlayerInfo.LastDeathReason = DeathReason.Exile;
                await _eventManager.CallAsync(new PlayerExileEvent(Game, Game.GetClientPlayer(exiled!.OwnerId)!, exiled));
            }

            await _eventManager.CallAsync(new MeetingEndedEvent(Game, this, exiled, tie, wasOverruled, overrideId));
        }
    }
}
