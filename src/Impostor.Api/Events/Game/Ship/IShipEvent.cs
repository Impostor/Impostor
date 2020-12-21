using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Ship
{
    public interface IShipEvent : IGameEvent
    {
        /// <summary>
        ///     Gets the <see cref="IInnerShipStatus"/> that triggered this <see cref="IShipEvent"/>
        /// </summary>
        IInnerShipStatus ShipStatus { get; }
    }
}
