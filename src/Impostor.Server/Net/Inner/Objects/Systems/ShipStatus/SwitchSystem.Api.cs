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
    internal partial class SwitchSystem : ISwitchSystem
    {
        IGame IShipSystem.Game => _game;

        public bool IsActive { get; internal set; }

        public byte ExpectedSwitches { get; internal set; }

        public byte ActualSwitches { get; internal set; }

        public byte Percentage { get; internal set; } = byte.MaxValue;

        public async ValueTask Start(byte startingPosition, byte expectedPosition)
        {
            await _game.GameNet.ShipStatus.SabotageSystem.SetCooldown(30.0f);

            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.Electrical); // electrical
            writer.Write(expectedPosition); // expected
            writer.Write(startingPosition); // current
            writer.Write((byte)0); // % complete

            IsActive = true;

            await _game.FinishDataMessageAsync(writer);
        }

        public async ValueTask Stop()
        {
            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.Electrical); // electrical
            writer.Write((byte)0); // expected
            writer.Write((byte)0); // current
            writer.Write(byte.MaxValue); // % complete

            IsActive = false;

            await _game.FinishDataMessageAsync(writer);
        }
    }
}
