using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    public class SecurityCameraSystemType : ISystemType
    {
        private readonly IEventManager _eventManager;
        private readonly IGame _game;

        public SecurityCameraSystemType(IEventManager eventManager, IGame game)
        {
            _game = game;
            _eventManager = eventManager;
        }

        public byte InUse { get; internal set; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask Deserialize(IMessageReader reader, bool initialState)
        {
            InUse = reader.ReadByte();

            return default;
        }
    }
}
