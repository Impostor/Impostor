using System;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
using Impostor.Server.Events.System;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class HudOverrideSystemType : ISystemType, IActivatable
    {
        // Next Two are probably to move into ISystemType. Maybe while replacing with an IInnerShipStatus for reporting events.
        private readonly IEventManager _eventManager;
        private readonly IGame _game;

        public HudOverrideSystemType(IGame game, IEventManager eventManager)
        {
            _game = game;
            _eventManager = eventManager;
        }

        public bool IsActive { get; private set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new NotImplementedException();
        }

        public async void Deserialize(IMessageReader reader, bool initialState)
        {
            bool status = reader.ReadBoolean();

            if (status != IsActive)
            {
                await _eventManager.CallAsync(new HudOverrideSystemEvent(_game, status));
            }

            IsActive = status;
        }
    }
}
