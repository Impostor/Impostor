using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Player
{
    public interface IPlayerExitVentEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets get the id of the used vent.
        /// </summary>
        public Vent Vent { get; }
    }
}
