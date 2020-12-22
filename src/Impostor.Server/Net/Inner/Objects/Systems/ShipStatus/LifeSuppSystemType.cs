using System.Collections.Generic;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class LifeSuppSystemType : ISystemType, IActivatable
    {
        private readonly IEventManager _eventManager;
        private readonly IGame _game;

        public LifeSuppSystemType(IEventManager eventManager, IGame game)
        {
            _eventManager = eventManager;
            _game = game;

            Countdown = 10000f;
            CompletedConsoles = new HashSet<int>();
        }

        public float Countdown { get; private set; }

        public HashSet<int> CompletedConsoles { get; }

        public bool IsActive => Countdown < 10000.0;

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask Deserialize(IMessageReader reader, bool initialState)
        {
            Countdown = reader.ReadSingle();

            if (reader.Position >= reader.Length)
            {
                return default;
            }

            CompletedConsoles.Clear(); // TODO: Thread safety

            var num = reader.ReadPackedInt32();

            for (var i = 0; i < num; i++)
            {
                CompletedConsoles.Add(reader.ReadPackedInt32());
            }

            return default;
        }
    }
}
