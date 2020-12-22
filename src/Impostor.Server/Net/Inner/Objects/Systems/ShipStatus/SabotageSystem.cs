using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    internal partial class SabotageSystem : ISystemType
    {
        private readonly IActivatable[] _specials;

        private readonly IEventManager _eventManager;
        private readonly Game _game;

        public SabotageSystem(IActivatable[] specials, IEventManager eventManager, Game game)
        {
            _specials = specials;
            _eventManager = eventManager;
            _game = game;
        }

        public float Timer { get; set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask Deserialize(IMessageReader reader, bool initialState)
        {
            Timer = reader.ReadSingle();

            return default;
        }
    }
}
