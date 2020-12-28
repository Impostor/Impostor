using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Ship
{
    /// <summary>
    ///     Called whenever the impostor closes doors
    /// </summary>
    public interface IShipDoorsCloseEvent : IShipEvent
    {
        /// <summary>
        /// Gets the doors which have been closed
        /// </summary>
        public SystemTypes SystemType { get; }
    }
}