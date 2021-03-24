namespace Impostor.Api.Events
{
    public interface IEventCancelable : IEvent
    {
        /// <summary>
        ///     Gets or sets a value indicating whether the event was cancelled.
        /// </summary>
        bool IsCancelled { get; set; }
    }
}
