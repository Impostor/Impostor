using Impostor.Api.Innersloth.Maps;

namespace Impostor.Api.Events.Player
{
    /// <summary>
    ///     Called whenever a player enters a vent.
    /// </summary>
    public interface IPlayerEnterVentEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the entered vent.
        /// </summary>
        public VentData Vent { get; }
    }
}
