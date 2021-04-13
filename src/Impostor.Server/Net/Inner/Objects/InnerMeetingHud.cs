using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api;
using Impostor.Api.Events.Managers;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
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

        [AllowNull]
        private PlayerVoteArea[] _playerStates;

        public InnerMeetingHud(Game game, ILogger<InnerMeetingHud> logger, IEventManager eventManager) : base(game)
        {
            _logger = logger;
            _eventManager = eventManager;
            _playerStates = null;

            Components.Add(this);
        }

        public byte ReporterId { get; private set; }

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
                PopulateButtons(0);

                foreach (var playerState in _playerStates)
                {
                    playerState.Deserialize(reader);

                    if (playerState.DidReport)
                    {
                        ReporterId = playerState.TargetPlayerId;
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
                        _playerStates[i].Deserialize(reader);
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
                    await HandleVotingComplete(sender, states, playerId, tie);
                    break;
                }

                case RpcCalls.CastVote:
                {
                    Rpc24CastVote.Deserialize(reader, out var playerId, out var suspectPlayerId);
                    return await HandleCastVote(sender, target, playerId, suspectPlayerId);
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
                    return await UnregisteredCall(call, sender);
            }

            return true;
        }

        private void PopulateButtons(byte reporter)
        {
            _playerStates = Game.GameNet.GameData!.Players
                .Select(x =>
                {
                    var area = new PlayerVoteArea(this, x.Key);
                    area.SetDead(x.Value.PlayerId == reporter, x.Value.Disconnected || x.Value.IsDead);
                    return area;
                })
                .ToArray();
        }

        private async ValueTask HandleVotingComplete(ClientPlayer sender, ReadOnlyMemory<byte> states, byte playerId, bool tie)
        {
            if (playerId != byte.MaxValue)
            {
                var player = Game.GameNet.GameData!.GetPlayerById(playerId);
                if (player?.Controller != null)
                {
                    player.Controller.Die(DeathReason.Exile);
                    await _eventManager.CallAsync(new PlayerExileEvent(Game, sender, player.Controller));
                }
            }

            await _eventManager.CallAsync(new MeetingEndedEvent(Game, this));
        }

        private async ValueTask<bool> HandleCastVote(ClientPlayer sender, ClientPlayer? target, byte playerId, sbyte suspectPlayerId)
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

            return true;
        }
    }
}
