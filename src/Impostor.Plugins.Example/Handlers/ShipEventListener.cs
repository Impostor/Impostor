using System;
using Impostor.Api.Events;
using Impostor.Api.Events.Ship;

namespace Impostor.Plugins.Example.Handlers
{
    public class ShipEventListener : IEventListener
    {
        [EventListener(EventPriority.Monitor)]
        public void OnGame(IShipEvent e)
        {
            Console.WriteLine(e.GetType().Name + " triggered");
        }

        [EventListener]
        public void OnDoorsClosed(IShipDoorsCloseEvent e)
        {
            Console.WriteLine("Ship > doors closed");
            Console.WriteLine("- " + e.SystemType);
        }
    }
}
