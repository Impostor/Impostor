namespace Impostor.Api.Events
{
    public interface ICancellableEvent : IEvent
    {
        /// <summary>
        /// Set to true to cancel the packet.
        /// </summary>
        bool Cancel { get; set; }
    }
}