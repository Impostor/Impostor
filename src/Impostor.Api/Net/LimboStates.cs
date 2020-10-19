using System;

namespace Impostor.Server.Net
{
    [Flags]
    public enum LimboStates
    {
        PreSpawn = 1,
        NotLimbo = 2,
        WaitingForHost = 4,
        All = PreSpawn | NotLimbo | WaitingForHost
    }
}