using Impostor.Api.Games;
using Impostor.Api.Net.Messages;

namespace Impostor.Server.GameData.Objects
{
    public class InnerLobbyBehaviour : InnerNetObject
    {
        private readonly IGame _game;

        public InnerLobbyBehaviour(IGame game)
        {
            _game = game;

            Components.Add(this);
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