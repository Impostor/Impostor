using System;
using System.Linq;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Data;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.Logging;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public partial class InnerMeetingHud : InnerNetObject
    {
        private readonly ILogger<InnerMeetingHud> _logger;
        private readonly IGame _game;
        private readonly IGameNet _gameNet;
        private PlayerVoteArea[] _playerStates;

        public InnerMeetingHud(ILogger<InnerMeetingHud> logger, IGame game)
        {
            _logger = logger;
            _game = game;
            _gameNet = game.GameNet;
            _playerStates = null;

            Components.Add(this);
        }

        public byte ReporterId { get; private set; }

        private void PopulateButtons(byte reporter)
        {
            _playerStates = _gameNet.GameData.Players
                .Select(x =>
                {
                    var area = new PlayerVoteArea(this, x.Key);
                    area.SetDead(x.Value.PlayerId == reporter, x.Value.Disconnected || x.Value.IsDead);
                    return area;
                })
                .ToArray();
        }

        public override void HandleRpc(IClientPlayer sender, IClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.Close:
                {
                    if (!sender.IsHost)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.Close)} but was not a host.");
                    }

                    if (target != null)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.Close)} to a specific player instead of broadcast.");
                    }

                    break;
                }

                case RpcCalls.VotingComplete:
                {
                    if (!sender.IsHost)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.VotingComplete)} but was not a host.");
                    }

                    if (target != null)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.VotingComplete)} to a specific player instead of broadcast.");
                    }

                    var states = reader.ReadBytesAndSize();
                    var playerId = reader.ReadByte();
                    var tie = reader.ReadBoolean();
                    break;
                }

                case RpcCalls.CastVote:
                {
                    var srcPlayerId = reader.ReadByte();
                    if (srcPlayerId != sender.Character.PlayerId)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.CastVote)} to an unowned {nameof(InnerPlayerControl)}.");
                    }

                    // Host broadcasts vote to others.
                    if (sender.IsHost && target != null)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.CastVote)} to a specific player instead of broadcast.");
                    }

                    // Player sends vote to host.
                    if (target == null || !target.IsHost)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.CastVote)} to wrong destinition, must be host.");
                    }

                    var targetPlayerId = reader.ReadByte();
                    break;
                }

                default:
                {
                    _logger.LogWarning("{0}: Unknown rpc call {1}", nameof(InnerMeetingHud), call);
                    break;
                }
            }
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IClientPlayer? target, IMessageReader reader, bool initialState)
        {
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
    }
}