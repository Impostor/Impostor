using System;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects.ShipSystems;
using Impostor.Api.Net.Messages;
using Impostor.Server.Events.Ship;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Logging;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    internal partial class SwitchSystem : ISystemType, IActivatable
    {
        private readonly IEventManager _eventManager;
        private readonly Game _game;

        public SwitchSystem(IEventManager eventManager, Game game)
        {
            _game = game;
            _eventManager = eventManager;
        }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask Deserialize(IMessageReader reader, bool initialState)
        {
            ExpectedSwitches = reader.ReadByte();
            ActualSwitches = reader.ReadByte();
            Percentage = reader.ReadByte();

            IsActive = Percentage == byte.MaxValue;

            await _eventManager.CallAsync(new ShipSwitchStateChangedEvent(_game, this));
        }
    }
}
