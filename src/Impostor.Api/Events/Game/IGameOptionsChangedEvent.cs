using Impostor.Api.Games;

namespace Impostor.Api.Events
{
    /// <summary>
    /// This event is triggered when game options are going to be changed.
    /// </summary>
    /// <remarks>
    /// Be careful when calling SyncSettingsAsync while handling this event,
    /// as it may call this event again in some cases.
    /// </remarks>
    public interface IGameOptionsChangedEvent : IGameEvent
    {
        /// <summary>
        /// Lists the possible reasons that the game options may have changed.
        /// </summary>
        public enum ChangeReason
        {
            /// <summary>
            /// The options were changed by the host using LogicOptions deserialization.
            /// </summary>
            /// <remarks>
            /// This event does not change the message sent to other players,
            /// so changes made to Game.Options are not synced until
            /// SyncSettingsAsync is called afterwards.
            /// </remarks>
            Host,

            /// <summary>
            /// The options were changed by a plugin using the
            /// SyncSettingsAsync method in <see cref="IGame"/>.
            /// </summary>
            /// <remarks>
            /// This event is called after serializing the settings, which
            /// means that changes made by event handlers will not be synced
            /// over.
            ///
            /// We strongly recommend against calling SyncSettingsAsync while
            /// handling this event.
            /// </remarks>
            Api,
        }

        /// <summary>
        /// Gets the reason that the game options have changed.
        /// </summary>
        public ChangeReason ChangedBy { get; }
    }
}
