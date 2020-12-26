namespace Impostor.Api.Events
{
    public interface IGameDoorStateChangedEvent : IGameEvent
    {
        public uint DoorMask { get; } // Mask is 2^doorID e.g. for Medbay (ID: 10), it's 1024

        public bool IsOpen { get; }
    }
}
