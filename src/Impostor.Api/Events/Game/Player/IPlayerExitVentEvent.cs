using Impostor.Api.Innersloth;

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
        public IVent Vent { get; }
    }
}
