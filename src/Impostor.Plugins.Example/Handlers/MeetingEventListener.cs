using System;
using Impostor.Api.Events;

namespace Impostor.Plugins.Example.Handlers
{
    public class MeetingEventListener : IEventListener
    {
        [EventListener]
        public void OnMeetingStarted(MeetingStartedEvent e)
        {
            Console.WriteLine("Meeting > started");
        }

        [EventListener]
        public void OnMeetingEnded(MeetingEndedEvent e)
        {
            Console.WriteLine("Meeting > ended");
        }
    }
}