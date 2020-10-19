namespace Impostor.Server.Events
{
    public interface IEventCancelable : IEvent
    {
        /// <summary>
        ///     True if the event was cancelled.
        /// </summary>
        bool IsCancelled { get; set; }
    }
}