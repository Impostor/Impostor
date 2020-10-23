using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Messages;

namespace Impostor.Api.Innersloth.Net.Objects
{
    public class InnerLobbyBehaviour : InnerNetObject
    {
        private readonly IGame _game;

        public InnerLobbyBehaviour(IGame game)
        {
            _game = game;

            Components.Add(this);
        }

        public override void HandleRpc(IClientPlayer sender, IClientPlayer? target, RpcCalls call, IMessageReader reader)
        {
            throw new System.NotImplementedException();
        }

        public override bool Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public override void Deserialize(IClientPlayer sender, IMessageReader reader, bool initialState)
        {
            throw new System.NotImplementedException();
        }
    }
}