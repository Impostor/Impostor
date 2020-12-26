using Impostor.Api.Events;
using Impostor.Api.Games;

namespace Impostor.Server.Events
{
    public class GameDoorStateChangedEvent : IGameDoorStateChangedEvent
    {
        public GameDoorStateChangedEvent(IGame game, uint mask, bool open)
        {
            Game = game;
            DoorMask = mask;
            IsOpen = open;
        }

        public IGame Game { get; }

        public uint DoorMask { get; }

        public bool IsOpen { get; }
    }
}
