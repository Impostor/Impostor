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
        }

        public override void HandleRpc(byte callId, IMessageReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(IMessageReader reader, bool initialState)
        {
            throw new System.NotImplementedException();
        }
    }
}