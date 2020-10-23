using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net.Objects.Components;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public partial class InnerGameData : InnerNetObject
    {
        private readonly IGame _game;
        private readonly ConcurrentDictionary<byte, PlayerInfo> _allPlayers;

        public InnerGameData(IGame game)
        {
            _game = game;
            _allPlayers = new ConcurrentDictionary<byte, PlayerInfo>();

            Components.Add(this);
            Components.Add(new InnerVoteBanSystem());
        }

        public int PlayerCount => _allPlayers.Count;

        public IReadOnlyDictionary<byte, PlayerInfo> Players => _allPlayers;

        internal void AddPlayer(InnerPlayerControl control)
        {
            var playerId = control.PlayerId;
            var playerInfo = new PlayerInfo(control.PlayerId);

            if (_allPlayers.TryAdd(playerId, playerInfo))
            {
                control.PlayerInfo = playerInfo;
            }
        }

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
            throw new NotImplementedException();
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IMessageReader reader, bool initialState)
        {
            if (initialState)
            {
                var num = reader.ReadPackedInt32();

                for (var i = 0; i < num; i++)
                {
                    var playerInfo = new PlayerInfo(reader.ReadByte());

                    playerInfo.Deserialize(reader);

                    if (!_allPlayers.TryAdd(playerInfo.PlayerId, playerInfo))
                    {
                        throw new ImpostorException("Failed to add player to InnerGameData.");
                    }
                }
            }
            else
            {
                throw new NotImplementedException("This shouldn't happen, according to Among Us disassembly..");

                // var num = reader.ReadByte();
                //
                // for (var i = 0; i < num; i++)
                // {
                //     var id = reader.ReadByte();
                //     var player = GetPlayerById(id);
                //     if (player != null)
                //     {
                //         player.Deserialize(reader);
                //     }
                //     else
                //     {
                //         var playerInfo = new PlayerInfo(id);
                //         playerInfo.Deserialize(reader);
                //         _allPlayers.Add(playerInfo);
                //     }
                // }
            }
        }
    }
}