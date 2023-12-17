using Impostor.Api.Innersloth.Maps;

namespace Impostor.Api.Events.Player
{
    /// <summary>
    ///     Called whenever a player moves to another vent.
    /// </summary>
    public interface IPlayerVentEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the vent player moved to.
        /// </summary>
        public VentData NewVent { get; }
    }
}
