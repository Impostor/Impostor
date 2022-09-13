using System;

namespace Impostor.Api.Innersloth
{
    [Flags]
    public enum MapFlags
    {
        Skeld = 1,
        MiraHQ = 2,
        Polus = 4,
        Airship = 16,

        // 8 is taken by Dleks
    }
}
