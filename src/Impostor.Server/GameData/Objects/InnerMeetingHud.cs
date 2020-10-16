using Impostor.Server.Games;
using Impostor.Server.Net.Messages;

namespace Impostor.Server.GameData.Objects
{
    public class InnerMeetingHud : InnerNetObject
    {
        private readonly IGame _game;

        public InnerMeetingHud(IGame game)
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