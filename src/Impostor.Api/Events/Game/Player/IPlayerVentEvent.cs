using Impostor.Api.Innersloth;

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
        public IVent NewVent { get; }
    }
}
