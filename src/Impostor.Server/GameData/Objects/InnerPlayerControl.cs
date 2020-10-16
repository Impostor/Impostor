using System;
using Impostor.Server.GameData.Objects.Components;
using Impostor.Server.Games;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.GameData.Objects
{
    public class InnerPlayerControl : InnerNetObject
    {
        private readonly IGame _game;

        public InnerPlayerControl(IGame game)
        {
            _game = game;

            Components.Add(this);
            Components.Add(new InnerPlayerPhysics());
            Components.Add(new InnerCustomNetworkTransform());

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
            // TODO: Might be unreliable, maybe we need to check if the length is 2 or 1.
            if (initialState)
            {
                IsNew = reader.ReadBoolean();
            }

            PlayerId = reader.ReadByte();
        }
    }
}