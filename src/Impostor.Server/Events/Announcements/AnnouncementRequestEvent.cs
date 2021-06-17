using Impostor.Api.Events.Announcements;
using Impostor.Api.Innersloth;

namespace Impostor.Server.Events.Announcements
{
    public class AnnouncementRequestEvent : IAnnouncementRequestEvent
    {
        public AnnouncementRequestEvent(int id, Language language)
        {
            Id = id;
            Language = language;
        }

        public int Id { get; }

        public Language Language { get; }

        public IAnnouncementRequestEvent.IResponse Response { get; set; } = new AnnouncementResponse();

        public class AnnouncementResponse : IAnnouncementRequestEvent.IResponse
        {
            public bool UseCached { get; set; } = false;

            public Announcement? Announcement { get; set; } = null;
        }
    }
}
