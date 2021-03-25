namespace Impostor.Server.Recorder
{
    internal enum RecordedPacketType : byte
    {
        Connect = 1,
        Disconnect = 2,
        Message = 3,
        GameCreated = 4,
    }
}
