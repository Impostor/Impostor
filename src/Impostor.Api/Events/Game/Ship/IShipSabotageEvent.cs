using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Ship
{
    /// <summary>
    ///     Called whenever the impostor triggers a sabotage
    /// </summary>
    public interface IShipSabotageEvent : IShipEvent
    {
        /// <summary>
        /// Gets the sabotage that was triggered
        /// </summary>
        public SystemTypes SystemType { get; }
    }
}