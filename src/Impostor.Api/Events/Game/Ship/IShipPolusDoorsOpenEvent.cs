using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Ship
{
    /// <summary>
    ///     Called whenever doors are opened on Polus
    /// </summary>
    public interface IShipPolusDoorsOpenEvent : IShipEvent
    {
        /// <summary>
        /// Gets the door which has been opened
        /// </summary>
        public PolusDoors Door { get; }
    }
}