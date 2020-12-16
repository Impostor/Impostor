using Impostor.Api.Events;
using Impostor.Api.Events.Announcements;
using Impostor.Api.Innersloth;

namespace Impostor.Plugins.Example.Handlers
{
    public class AnnouncementsListener : IEventListener
    {
        private const int Id = 50;

        [EventListener]
        public void OnAnnouncementRequestEvent(IAnnouncementRequestEvent e)
        {
            if (e.Id == Id)
            {
                e.Response.UseCached = true;
            }
            else
            {
                e.Response.Announcement = new Announcement(Id, "Hello!");
            }
        }
    }
}
