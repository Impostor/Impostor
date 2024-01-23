using System.Collections.Generic;
using Impostor.Api.Config;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Custom;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Net.Inner.Objects.ShipStatus
{
    internal class InnerFungleShipStatus : InnerShipStatus
    {
        public InnerFungleShipStatus(ICustomMessageManager<ICustomRpc> customMessageManager, IOptions<AntiCheatConfig> antiCheatConfig, Game game) : base(customMessageManager, antiCheatConfig, game, MapTypes.Fungle)
        {
        }

        protected override void AddSystems(Dictionary<SystemTypes, ISystemType> systems)
        {
            base.AddSystems(systems);

            systems.Add(SystemTypes.Comms, new HudOverrideSystemType());
            systems.Add(SystemTypes.Reactor, new ReactorSystemType());
            systems.Add(SystemTypes.Doors, new DoorsSystemType(Doors));
            systems.Add(SystemTypes.MushroomMixupSabotage, new MushroomMixupSabotageSystemType());
        }
    }
}
