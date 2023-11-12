using Impostor.Api.Innersloth;

namespace Impostor.Api;

public static class MurderResultFlagsExtensions
{
    public static bool IsFailed(this MurderResultFlags value)
    {
        return (value & (MurderResultFlags.FailedError | MurderResultFlags.FailedProtected)) != 0;
    }
}
