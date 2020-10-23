using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net.Objects.Components;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public partial class InnerGameData : InnerNetObject
    {
        private readonly ILogger<InnerGameData> _logger;
        private readonly IGame _game;
        private readonly ConcurrentDictionary<byte, PlayerInfo> _allPlayers;

        public InnerGameData(ILogger<InnerGameData> logger, IGame game, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _game = game;
            _allPlayers = new ConcurrentDictionary<byte, PlayerInfo>();

            Components.Add(this);
            Components.Add(ActivatorUtilities.CreateInstance<InnerVoteBanSystem>(serviceProvider));
        }

        public int PlayerCount => _allPlayers.Count;

        public IReadOnlyDictionary<byte, PlayerInfo> Players => _allPlayers;

        public PlayerInfo? GetPlayerById(byte id)
        {
            if (id == byte.MaxValue)
            {
                return null;
            }

            return _allPlayers.TryGetValue(id, out var player) ? player : null;
        }

        public override void HandleRpc(IClientPlayer sender, IClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            switch (call)
            {
                case RpcCalls.SetTasks:
                {
                    if (!sender.IsHost)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.SetTasks)} but was not a host.");
                    }

                    if (target != null)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.SetTasks)} to a specific player instead of broadcast.");
                    }

                    var playerId = reader.ReadByte();
                    var taskTypeIds = reader.ReadBytesAndSize();

                    SetTasks(playerId, taskTypeIds);
                    break;
                }

                case RpcCalls.UpdateGameData:
                {
                    if (!sender.IsHost)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.SetTasks)} but was not a host.");
                    }

                    if (target != null)
                    {
                        throw new ImpostorCheatException($"Client sent {nameof(RpcCalls.SetTasks)} to a specific player instead of broadcast.");
                    }

                    while (reader.Position < reader.Length)
                    {
                        var message = reader.ReadMessage();
                        var player = GetPlayerById(message.Tag);
                        if (player != null)
                        {
                            player.Deserialize(message);
                        }
                        else
                        {
                            var playerInfo = new PlayerInfo(message.Tag);

                            playerInfo.Deserialize(reader);

                            if (!_allPlayers.TryAdd(playerInfo.PlayerId, playerInfo))
                            {
                                throw new ImpostorException("Failed to add player to InnerGameData.");
                            }
                        }
                    }

                    break;
                }

                default:
                {
                    _logger.LogWarning("{0}: Unknown rpc call {1}", nameof(InnerGameData), call);
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
                var num = reader.ReadPackedInt32();

                for (var i = 0; i < num; i++)
                {
                    var playerId = reader.ReadByte();
                    var playerInfo = new PlayerInfo(playerId);

                    playerInfo.Deserialize(reader);

                    if (!_allPlayers.TryAdd(playerInfo.PlayerId, playerInfo))
                    {
                        throw new ImpostorException("Failed to add player to InnerGameData.");
                    }
                }
            }
            else
            {
                throw new NotImplementedException("This shouldn't happen, according to Among Us disassembly.");
            }
        }

        internal void AddPlayer(InnerPlayerControl control)
        {
            var playerId = control.PlayerId;
            var playerInfo = new PlayerInfo(control.PlayerId);

            if (_allPlayers.TryAdd(playerId, playerInfo))
            {
                control.PlayerInfo = playerInfo;
            }
        }

        private void SetTasks(byte playerId, ReadOnlyMemory<byte> taskTypeIds)
        {
            var player = GetPlayerById(playerId);
            if (player == null)
            {
                _logger.LogTrace("Could not set tasks for playerId {0}.", playerId);
                return;
            }

            if (player.Disconnected)
            {
                return;
            }

            player.Tasks = new List<TaskInfo>(taskTypeIds.Length);

            for (var i = 0; i < taskTypeIds.Length; i++)
            {
                player.Tasks.Add(new TaskInfo());
                player.Tasks[i].Id = (uint)i;
            }
        }
    }
}