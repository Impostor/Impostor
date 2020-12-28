using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
using Impostor.Server.Events.Ship;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    internal partial class LifeSuppSystemType : ISystemType, IActivatable
    {
        private readonly IEventManager _eventManager;
        private readonly Game _game;

        public LifeSuppSystemType(IEventManager eventManager, Game game)
        {
            _eventManager = eventManager;
            _game = game;

            Countdown = 10000f;
            CompletedConsoles = new HashSet<int>();
        }

        public HashSet<int> CompletedConsoles { get; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask Deserialize(IMessageReader reader, bool initialState)
        {
            Countdown = reader.ReadSingle();

            if (reader.Position >= reader.Length)
            {
                return;
            }

            CompletedConsoles.Clear(); // TODO: Thread safety

            var num = reader.ReadPackedInt32();

            for (var i = 0; i < num; i++)
            {
                CompletedConsoles.Add(reader.ReadPackedInt32());
            }

            await _eventManager.CallAsync(new ShipOxygenStateChangedEvent(_game, this));
        }
    }
}
