using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Server.Net.Inner.Objects.Systems.ShipStatus
{
    internal partial class LifeSuppSystemType : IOxygenSystem
    {
        IGame IShipSystem.Game => _game;

        public bool IsActive => Countdown < 10000.0;

        public float Countdown { get; private set; }

        IEnumerable<int> IOxygenSystem.CompletedConsoles => CompletedConsoles;

        public async ValueTask Start(float time)
        {
            await _game.GameNet.ShipStatus.GetSabotageSystem().SetCooldown(30.0f);

            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.LifeSupp); // oxygen
            writer.Write(time); // state
            writer.WritePacked(0); // Number of completed consoles

            await _game.FinishDataMessageAsync(writer);

            Countdown = time;
            CompletedConsoles.Clear();
        }

        public async ValueTask Stop()
        {
            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.LifeSupp); // oxygen
            writer.Write(10000f); // state
            writer.WritePacked(0); // Number of completed consoles

            await _game.FinishDataMessageAsync(writer);

            Countdown = 10000f;
            CompletedConsoles.Clear();
        }
    }
}
