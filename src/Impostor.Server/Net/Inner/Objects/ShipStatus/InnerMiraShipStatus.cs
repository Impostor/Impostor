using System.Collections.Generic;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects.ShipStatus;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.ShipStatus;

internal class InnerMiraShipStatus(Game game) : InnerShipStatus(game, MapTypes.MiraHQ), IInnerMiraShipStatus
{
    protected override void AddSystems(Dictionary<SystemTypes, ISystemType> systems)
    {
        base.AddSystems(systems);

        systems.Add(SystemTypes.Comms, new HudOverrideSystemType());
        systems.Add(SystemTypes.Reactor, new ReactorSystemType());
        systems.Add(SystemTypes.LifeSupp, new LifeSuppSystemType());
    }
}
