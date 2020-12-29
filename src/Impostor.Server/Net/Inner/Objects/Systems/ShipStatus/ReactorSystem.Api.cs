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
            await _game.GameNet.ShipStatus.SabotageSystem.SetCooldown(30.0f);

            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            if (_game.Options.Map != MapTypes.Polus)
            {
                writer.WritePacked(1 << (int)SystemTypes.Reactor);
                writer.Write(time < 0 ? 30.0f : time); // time
            }
            else
            {
                writer.WritePacked(1 << (int)SystemTypes.Laboratory);
                writer.Write(time < 0 ? 60.0f : time); // time
            }

            writer.WritePacked(0); // Number of players holding buttons

            await _game.FinishDataMessageAsync(writer);

            Countdown = time;
            UserConsolePairs.Clear();
        }

        public async ValueTask Stop()
        {
            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.Reactor);
            writer.Write(10000f); // time
            writer.WritePacked(0); // Number of players holding buttons

            await _game.FinishDataMessageAsync(writer);

            Countdown = 10000f;
            UserConsolePairs.Clear();
        }
    }
}
