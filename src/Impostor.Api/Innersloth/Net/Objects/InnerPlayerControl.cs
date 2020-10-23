using System;
using Impostor.Api.Games;
using Impostor.Api.Innersloth.Net.Objects.Components;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public class InnerPlayerControl : InnerNetObject
    {
        private readonly IGame _game;

        public InnerPlayerControl(IGame game, IServiceProvider serviceProvider)
        {
            _game = game;

            Components.Add(this);
            Components.Add(ActivatorUtilities.CreateInstance<InnerPlayerPhysics>(serviceProvider));
            Components.Add(ActivatorUtilities.CreateInstance<InnerCustomNetworkTransform>(serviceProvider));

            PlayerId = byte.MaxValue;
        }

        public bool IsNew { get; private set; }

        public byte PlayerId { get; private set; }

        public InnerGameData.PlayerInfo PlayerInfo { get; internal set; }

        public override void HandleRpc(IClientPlayer sender, byte callId, IMessageReader reader)
        {
            switch (callId)
            {
                case 3:
                {
                    var length = reader.ReadPackedInt32();

                    for (var i = 0; i < length; i++)
                    {
                        var playerId = reader.ReadByte();
                        var player = _game.GameNet.GameData.GetPlayerById(playerId);

                        player.IsImpostor = true;
                    }

                    break;
                }

                case 6:
                {
                    PlayerInfo.PlayerName = reader.ReadString();
                    break;
                }

                default:
                    break;
            }
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IMessageReader reader, bool initialState)
        {
            if (initialState)
            {
                IsNew = reader.ReadBoolean();
            }

            PlayerId = reader.ReadByte();
        }
    }
}