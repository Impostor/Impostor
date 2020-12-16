using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.Announcements
{
    public interface IAnnouncementRequestEvent : IEvent
    {
        public interface IResponse
        {
            public FreeWeekendState FreeWeekendState { get; set; }

            public bool UseCached { get; set; }

            public Announcement? Announcement { get; set; }
        }

        public int Id { get; }

        public Language Language { get; }

        public IResponse Response { get; set; }
    }
}
