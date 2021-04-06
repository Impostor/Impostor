using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerVentEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets get the id of the used vent.
        /// </summary>
        public Vent Vent { get; }

        /// <summary>
        ///     Gets a value indicating whether the vent was entered or exited.
        /// </summary>
        public bool VentEnter { get; }
    }
}
