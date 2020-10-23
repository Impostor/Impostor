using System;
using System.Linq;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Data;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public partial class InnerMeetingHud : InnerNetObject
    {
        private readonly IGame _game;
        private readonly GameNet _gameNet;
        private PlayerVoteArea[] _playerStates;

        public InnerMeetingHud(IGame game)
        {
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

        public override void HandleRpc(IClientPlayer sender, byte callId, IMessageReader reader)
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