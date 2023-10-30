using System;

namespace Impostor.Api.Innersloth
{
    [Flags]
    public enum MapFlags
    {
        Skeld = 1 << 0,
        MiraHQ = 1 << 1,
        Polus = 1 << 2,
        Dleks = 1 << 3,
        Airship = 1 << 4,
        Fungle = 1 << 5,
    }
}
