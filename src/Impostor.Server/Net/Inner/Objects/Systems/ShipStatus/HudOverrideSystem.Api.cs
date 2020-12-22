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
    internal partial class HudOverrideSystem : ICommsSystem
    {
        IGame IShipSystem.Game => _game;

        bool IActivable.IsActive => IsActive;

        public async ValueTask Start()
        {
            await _game.GameNet.ShipStatus.GetSabotageSystem().SetCooldown(30.0f);

            // TODO: Use serialize (?)
            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.Comms); // comms
            writer.Write(true); // state

            await _game.FinishDataMessageAsync(writer);

            IsActive = true;
        }

        public async ValueTask Stop()
        {
            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.Comms); // comms
            writer.Write(false); // state

            await _game.FinishDataMessageAsync(writer);

            IsActive = false;
        }
    }
}
