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
        public void OnPolusDoorOpened(IShipPolusDoorOpenEvent e)
        {
            Console.WriteLine("Ship - door opened");
            Console.WriteLine("- " + e.Door);
        }

        [EventListener]
        public void OnDecontamDoorOpened(IShipDecontamDoorOpenEvent e)
        {
            Console.WriteLine("Ship - decontam door opened");
            Console.WriteLine("- " + e.DecontamDoor);
        }
    }
}
