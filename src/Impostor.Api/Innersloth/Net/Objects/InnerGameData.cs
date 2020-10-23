using System;
using System.Collections.Generic;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net.Objects.Components;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public partial class InnerGameData : InnerNetObject
    {
        private readonly IGame _game;
        private readonly List<PlayerInfo> _allPlayers;

        public InnerGameData(IGame game)
        {
            _game = game;
            _allPlayers = new List<PlayerInfo>();

            Components.Add(this);
            Components.Add(new InnerVoteBanSystem());
        }

        public int PlayerCount => _allPlayers.Count;

        public IReadOnlyList<PlayerInfo> Players => _allPlayers;

        public PlayerInfo GetPlayerById(byte id)
        {
            foreach (var player in _allPlayers)
            {
                if (player.PlayerId == id)
                {
                    return player;
                }
            }

            return null;
        }

        public override void HandleRpc(byte callId, IMessageReader reader)
        {
            throw new NotImplementedException();
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IMessageReader reader, bool initialState)
        {
            if (initialState)
            {
                var num = reader.ReadPackedInt32();

                for (var i = 0; i < num; i++)
                {
                    var playerInfo = new PlayerInfo(reader.ReadByte());
                    playerInfo.Deserialize(reader);
                    _allPlayers.Add(playerInfo);
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