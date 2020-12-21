using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Net.Inner.Objects.ShipSystems;
using Impostor.Api.Net.Messages;
using Impostor.Server.Events.Ship;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    internal partial class HudOverrideSystem : ISystemType, IActivatable
    {
        private readonly IEventManager _eventManager;
        private readonly Game _game;

        public HudOverrideSystem(IEventManager eventManager, Game game)
        {
            _game = game;
            _eventManager = eventManager;
        }

        public bool IsActive { get; private set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask Deserialize(IMessageReader reader, bool initialState)
        {
            IsActive = reader.ReadBoolean();

            await _eventManager.CallAsync(new ShipCommsStateChangedEvent(_game, this));
        }
    }
}
