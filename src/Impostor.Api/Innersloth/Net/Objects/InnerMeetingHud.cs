using System;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Data;
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
            _playerStates = new PlayerVoteArea[_gameNet.GameData.PlayerCount];

            for (var i = 0; i < _playerStates.Length; i++)
            {
                var player = _gameNet.GameData.Players[i];
                var playerVoteArea = _playerStates[i] = new PlayerVoteArea(this, player.PlayerId);

                playerVoteArea.SetDead(player.PlayerId == reporter, player.Disconnected || player.IsDead);
            }
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