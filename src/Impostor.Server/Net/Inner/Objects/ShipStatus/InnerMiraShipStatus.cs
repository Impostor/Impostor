using System.Collections.Generic;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Custom;
using Impostor.Api.Net.Inner.Objects.ShipStatus;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.ShipStatus
{
    internal class InnerMiraShipStatus : InnerShipStatus, IInnerMiraShipStatus
    {
        public InnerMiraShipStatus(ICustomMessageManager<ICustomRpc> customMessageManager, ICustomMessageManager<ICustomSystemType> customSystemManager, Game game) : base(customMessageManager, customSystemManager, game, MapTypes.MiraHQ)
        {
            InitializeSystems();
        }

        protected override void AddSystems(Dictionary<SystemTypes, ISystemType> systems)
        {
            base.AddSystems(systems);

            systems.Add(SystemTypes.Comms, new HqHudSystemType());
            systems.Add(SystemTypes.Reactor, new ReactorSystemType(45f, SystemTypes.Reactor));
            systems.Add(SystemTypes.LifeSupp, new LifeSuppSystemType(45f));
            systems.Add(SystemTypes.Ventilation, new VentilationSystemType());
            systems.Add(SystemTypes.Decontamination, new DeconSystemType());
            systems.Add(SystemTypes.Decontamination2, new DeconSystemType());
        }
    }
}
