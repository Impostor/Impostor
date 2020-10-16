using Impostor.Server.GameData.Objects.Components;
using Impostor.Server.Games;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.GameData.Objects
{
    public class InnerGameData : InnerNetObject
    {
        private readonly IGame _game;

        public InnerGameData(IGame game)
        {
            _game = game;

            Components.Add(this);
            Components.Add(new InnerVoteBanSystem());
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