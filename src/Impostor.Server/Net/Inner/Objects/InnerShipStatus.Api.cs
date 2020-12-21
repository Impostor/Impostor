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
        public ICommsSystem GetCommsSystem()
        {
            return (ICommsSystem)_systems[SystemTypes.Comms];
        }

        public IReactorSystem GetReactorSystem()
        {
            return (IReactorSystem)_systems[SystemTypes.Reactor];
        }
    }
}
