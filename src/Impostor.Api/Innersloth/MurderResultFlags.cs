using System;

namespace Impostor.Api.Innersloth
{
    [Flags]
    public enum MurderResultFlags
    {
        Succeeded = 1,
        FailedError = 2,
        FailedProtected = 4,
        DecisionByHost = 8,
    }
}
