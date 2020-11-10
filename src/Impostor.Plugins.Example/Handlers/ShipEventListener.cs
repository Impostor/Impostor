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
        public void OnSabotage(IShipSabotageEvent e)
        {
            Console.WriteLine("Ship > sabotage");
            Console.WriteLine("- " + e.SystemType);
        }
        
        [EventListener]
        public void OnDoorsClosed(IShipDoorsCloseEvent e)
        {
            Console.WriteLine("Ship > doors closed");
            Console.WriteLine("- " + e.SystemType);
        }

        [EventListener]
        public void OnPolusDoorOpened(IShipPolusDoorsOpenEvent e)
        {
            Console.WriteLine("Ship - door opened");
            Console.WriteLine("- " + e.Door);
        }
    }
}
