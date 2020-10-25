namespace Impostor.Api.Events
{
    public interface IEventCancelable : IEvent
    {
        /// <summary>
        ///     True if the event was cancelled.
        /// </summary>
        bool IsCancelled { get; set; }
    }
}