using Impostor.Api.Innersloth.Maps;

namespace Impostor.Api.Events.Player
{
    /// <summary>
    ///     Called whenever a player exits a vent.
    /// </summary>
    public interface IPlayerExitVentEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the exited vent.
        /// </summary>
        public VentData Vent { get; }
    }
}
