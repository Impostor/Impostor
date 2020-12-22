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
    internal partial class SabotageSystem : ISabotageSystem
    {
        public IGame Game => _game;

        public async ValueTask SetCooldown(float time)
        {
            using var writer = _game.StartDataMessage(_game.GameNet.ShipStatus.NetId);

            writer.WritePacked(1 << (int)SystemTypes.Sabotage); // sabotage cooldown
            writer.Write(time); // timer

            await _game.FinishDataMessageAsync(writer);
        }
    }
}
