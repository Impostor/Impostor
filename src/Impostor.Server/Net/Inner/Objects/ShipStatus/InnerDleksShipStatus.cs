using System.Collections.Generic;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects.ShipStatus;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.ShipStatus;

internal class InnerDleksShipStatus(ICustomMessageManager<ICustomRpc> customMessageManager, Game game)
    : InnerShipStatus(customMessageManager, game, MapTypes.Dleks), IInnerDleksShipStatus
{
    protected override void AddSystems(Dictionary<SystemTypes, ISystemType> systems)
    {
        base.AddSystems(systems);

        systems.Add(SystemTypes.Doors, new AutoDoorsSystemType(Doors));
        systems.Add(SystemTypes.Comms, new HudOverrideSystemType());
        systems.Add(SystemTypes.Security, new SecurityCameraSystemType());
        systems.Add(SystemTypes.Reactor, new ReactorSystemType());
        systems.Add(SystemTypes.LifeSupp, new LifeSuppSystemType());
    }
}
