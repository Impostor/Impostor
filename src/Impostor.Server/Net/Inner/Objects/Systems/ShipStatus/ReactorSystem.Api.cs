using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    internal partial class ReactorSystem : IReactorSystem
    {
        IGame IShipSystem.Game => _game;

        public bool IsActive => Countdown < 10000.0;

        public float Countdown { get; private set; }

        IEnumerable<Tuple<IClientPlayer, byte>> IReactorSystem.UserConsolePairs => UserConsolePairs;

        public async ValueTask Start(float time)
        {
            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.Reactor); // comms
            writer.Write(time); // state
            writer.WritePacked(0); // Number of players holding buttons

            await _game.FinishDataMessageAsync(writer);

            Countdown = time;
            UserConsolePairs.Clear();
        }

        public async ValueTask Stop()
        {
            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.Reactor); // comms
            writer.Write(10000f); // state
            writer.WritePacked(0); // Number of players holding buttons

            await _game.FinishDataMessageAsync(writer);

            Countdown = 10000f;
            UserConsolePairs.Clear();
        }
    }
}
