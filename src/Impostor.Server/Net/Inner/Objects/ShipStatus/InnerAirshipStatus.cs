using System.Collections.Generic;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects.ShipStatus;
using Impostor.Server.Net.Inner.Objects.Systems;
using Impostor.Server.Net.Inner.Objects.Systems.ShipStatus;
using Impostor.Server.Net.State;

namespace Impostor.Server.Net.Inner.Objects.ShipStatus
{
    internal class InnerAirshipStatus : InnerShipStatus, IInnerAirshipStatus
    {
        public InnerAirshipStatus(Game game) : base(game)
        {
        }

        public override Dictionary<int, bool> Doors { get; } = new Dictionary<int, bool>(21);

        protected override void AddSystems(Dictionary<SystemTypes, ISystemType> systems)
        {
            base.AddSystems(systems);

            systems.Add(SystemTypes.Doors, new DoorsSystemType(Doors));
            systems.Add(SystemTypes.Comms, new HudOverrideSystemType());
            systems.Add(SystemTypes.GapRoom, new MovingPlatformBehaviour());
            systems.Add(SystemTypes.Reactor, new HeliSabotageSystemType());
            systems.Add(SystemTypes.Decontamination, new ElectricalDoors(Doors));
            systems.Add(SystemTypes.Decontamination2, new AutoDoorsSystemType(Doors));
            systems.Add(SystemTypes.Security, new SecurityCameraSystemType());
        }
    }
}
