using System;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
using Impostor.Server.GameData.Objects.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Impostor.Server.GameData.Objects
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
                IsNew = reader.ReadBoolean();
            }

            PlayerId = reader.ReadByte();
        }
    }
}