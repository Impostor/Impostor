using System;

namespace Impostor.Shared.Innersloth.Data
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