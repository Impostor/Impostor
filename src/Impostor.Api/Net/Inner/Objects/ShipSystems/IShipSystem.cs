using System;
using System.Collections.Generic;
using System.Text;
using Impostor.Api.Games;

namespace Impostor.Api.Net.Inner.Objects.ShipSystems
{
    public interface IShipSystem
    {
        IGame Game { get; }
    }
}
