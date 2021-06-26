using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Announcements
{
    /// <summary>
    ///     Event fired after client requests a announcement.
    /// </summary>
    public interface IAnnouncementRequestEvent : IEvent
    {
        public interface IResponse
        {
            /// <summary>
            ///     Gets or sets a value indicating whether announcement should be loaded from client's cache, can save some bytes.
            /// </summary>
            public bool UseCached { get; set; }

            /// <summary>
            ///     Gets or sets announcement, should be null when <see cref="UseCached" /> is set to true.
            /// </summary>
            public Announcement? Announcement { get; set; }
        }

        /// <summary>
        ///     Gets client's last announcement id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     Gets client's language.
        /// </summary>
        public Language Language { get; }

        /// <summary>
        ///     Gets or sets plugin made response.
        /// </summary>
        public IResponse Response { get; set; }
    }
}
