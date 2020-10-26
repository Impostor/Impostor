using System;
using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;

namespace Impostor.Plugins.Example.Handlers
{
    public class MeetingEventListener : IEventListener
    {
        [EventListener]
        public void OnMeetingStarted(IMeetingStartedEvent e)
        {
            Console.WriteLine("Meeting > started");
        }

        [EventListener]
        public void OnMeetingEnded(IMeetingEndedEvent e)
        {
            Console.WriteLine("Meeting > ended");
        }
    }
}
