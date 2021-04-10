using Impostor.Api.Innersloth;

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
        public IVent Vent { get; }
    }
}
