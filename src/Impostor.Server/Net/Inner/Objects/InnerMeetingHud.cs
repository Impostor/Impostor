using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Events.Player;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner;
using Impostor.Api.Net.Messages;
using Impostor.Api.Net.Messages.Rpcs;
using Impostor.Server.Events.Meeting;
using Impostor.Server.Events.Player;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerMeetingHud : InnerNetObject
    {
        private readonly ILogger<InnerMeetingHud> _logger;
        private readonly IEventManager _eventManager;

        private readonly CancellationTokenSource _timerToken;

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
                    await Task.Delay(TimeSpan.FromSeconds(Game.Options.DiscussionTime + Game.Options.VotingTime), _timerToken.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                await HandleVotingCompleteAsync();
            });
        }

        public InnerPlayerInfo? Reporter { get; private set; }

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

                foreach (var playerState in _playerStates)
                {
                    playerState.Deserialize(reader, false);

                    if (playerState.DidReport)
                    {
                        Reporter = playerState.TargetPlayer;
                    }
                }
            }
            else
            {
                var num = reader.ReadPackedUInt32();

                for (var i = 0; i < _playerStates.Length; i++)
                {
                    if ((num & 1 << i) != 0)
                    {
                        var playerVoteArea = _playerStates[i];

                        var clientPlayer = Game.Players.SingleOrDefault(x => x.Character?.PlayerId == playerVoteArea.TargetPlayer.PlayerId);
                        var isHost = (clientPlayer?.IsHost ?? false) && playerVoteArea.VoteType != VoteType.ForceSkip;

                        playerVoteArea.Deserialize(reader, isHost);

                        if (isHost)
                        {
                            await HandleVoteAsync(playerVoteArea);
                            await CheckForEndVotingAsync();
                        }
                    }
                }
            }
        }

        public override async ValueTask<bool> HandleRpcAsync(ClientPlayer sender, ClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.Close:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc22Close.Deserialize(reader);
                    break;
                }

                case RpcCalls.VotingComplete:
                {
                    if (!await ValidateHost(call, sender))
                    {
                        return false;
                    }

                    Rpc23VotingComplete.Deserialize(reader, out var states, out var playerId, out var tie);

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

        private async ValueTask<bool> HandleCastVoteAsync(ClientPlayer sender, ClientPlayer? target, byte playerId, sbyte suspectPlayerId)
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
                if (await sender.Client.ReportCheatAsync(RpcCalls.CastVote, $"Client sent {nameof(RpcCalls.CastVote)} to an unowned {nameof(InnerPlayerControl)}"))
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

        private async ValueTask CheckForEndVotingAsync()
        {
            if (_playerStates.All(ps => ps.IsDead || ps.DidVote))
            {
                await HandleVotingCompleteAsync();
            }
        }

        private byte[] CalculateVotes()
        {
            byte[] array = new byte[_playerStates.Max(x => x.TargetPlayer.PlayerId) + 2];
            foreach (var playerVoteArea in _playerStates)
            {
                if (playerVoteArea.DidVote)
                {
                    var index = playerVoteArea.VotedForId + 1;
                    if (index >= 0 && index < array.Length)
                    {
                        array[index] += 1;
                    }
                }
            }

            return array;
        }

        private int IndexOfMax<T>(T[] self, Func<T, int> comparer, out bool tie)
        {
            tie = false;
            var num = int.MinValue;
            var result = -1;
            for (var i = 0; i < self.Length; i++)
            {
                var num2 = comparer.Invoke(self[i]);
                if (num2 > num)
                {
                    result = i;
                    num = num2;
                    tie = false;
                }
                else if (num2 == num)
                {
                    tie = true;
                    result = -1;
                }
            }

            return result;
        }

        private async ValueTask HandleVotingCompleteAsync()
        {
            _timerToken.Cancel();

            foreach (var playerVoteArea in _playerStates)
            {
                if (!playerVoteArea.DidVote)
                {
                    playerVoteArea.SetVotedFor((sbyte)VoteType.ForceSkip);
                    await HandleVoteAsync(playerVoteArea);
                }
            }

            byte[] self = this.CalculateVotes();
            var maxIdx = IndexOfMax(self, p => p, out var tie) - 1;
            var exiled = Game.GameNet.GameData!.GetPlayerById((byte)maxIdx)?.Controller;

            if (exiled != null)
            {
                exiled.Die(DeathReason.Exile);
                await _eventManager.CallAsync(new PlayerExileEvent(Game, Game.GetClientPlayer(exiled!.OwnerId)!, exiled));
            }

            await _eventManager.CallAsync(new MeetingEndedEvent(Game, this, exiled, tie));
        }
    }
}
