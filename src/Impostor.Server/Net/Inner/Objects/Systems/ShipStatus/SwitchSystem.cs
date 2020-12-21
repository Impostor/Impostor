using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner.Objects.ShipSystems;
using Impostor.Api.Net.Messages;
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

        public byte ExpectedSwitches { get; set; }

        public byte ActualSwitches { get; set; }

        public byte Value { get; set; } = byte.MaxValue;

        public bool IsActive { get; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask Deserialize(IMessageReader reader, bool initialState)
        {
            ExpectedSwitches = reader.ReadByte();
            ActualSwitches = reader.ReadByte();
            Value = reader.ReadByte();

            return default;
        }
    }
}
