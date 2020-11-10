using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace Impostor.Api.Events.Ship
{
    public interface IShipEvent : IGameEvent
    {
        /// <summary>
        ///     Gets the <see cref="IClientPlayer"/> that triggered this <see cref="IShipEvent"/>.
        /// </summary>
        IClientPlayer ClientPlayer { get; }

        /// <summary>
        ///     Gets the networked <see cref="IInnerShipStatus"/> that triggered this <see cref="IShipEvent"/>.
        ///     This <see cref="IInnerShipStatus"/> belongs to the <see cref="IClientPlayer"/>.
        /// </summary>
        IInnerShipStatus ShipStatus { get; }
    }
}