using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Impostor.Api.Innersloth;
using Impostor.Api.Net.Inner.Objects;
using Impostor.Api.Net.Inner.Objects.ShipSystems;

namespace Impostor.Server.Net.Inner.Objects
{
    internal partial class InnerShipStatus : IInnerShipStatus
    {
        public ICommsSystem CommsSystem => (ICommsSystem)_systems[SystemTypes.Comms];

        public IReactorSystem ReactorSystem => (IReactorSystem)_systems[SystemTypes.Reactor];

        public ISabotageSystem SabotageSystem => (ISabotageSystem)_systems[SystemTypes.Sabotage];

        public ISwitchSystem SwitchSystem => (ISwitchSystem)_systems[SystemTypes.Electrical];

        public IOxygenSystem OxygenSystem => (IOxygenSystem)_systems[SystemTypes.LifeSupp];
    }
}
