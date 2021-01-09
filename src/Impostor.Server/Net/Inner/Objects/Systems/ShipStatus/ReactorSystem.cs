using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Messages;
using Impostor.Server.Events.Ship;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    internal partial class ReactorSystem : ISystemType, IActivatable
    {
        private readonly IEventManager _eventManager;
        private readonly Game _game;

        public ReactorSystem(IEventManager eventManager, Game game)
        {
            _eventManager = eventManager;
            _game = game;

            Countdown = 10000f;
            UserConsolePairs = new HashSet<Tuple<IClientPlayer, byte>>();
        }

        public HashSet<Tuple<IClientPlayer, byte>> UserConsolePairs { get; }

        public void Serialize(IMessageWriter writer, bool initialState)
        {
            throw new System.NotImplementedException();
        }

        public async ValueTask Deserialize(IMessageReader reader, bool initialState)
        {
            Countdown = reader.ReadSingle();
            UserConsolePairs.Clear(); // TODO: Thread safety

            var count = reader.ReadPackedInt32();

            for (var i = 0; i < count; i++)
            {
                var playerId = reader.ReadByte();
                var consoleId = reader.ReadByte();

                var player = _game.Players.Where((player) => player?.Character?.PlayerId == playerId).FirstOrDefault();

                if (player != null)
                {
                    UserConsolePairs.Add(new Tuple<IClientPlayer, byte>(player, consoleId));
                }
            }

            await _eventManager.CallAsync(new ShipReactorStateChangedEvent(_game, this));
        }
    }
}
