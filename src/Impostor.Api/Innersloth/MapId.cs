using System;

namespace Impostor.Api.Innersloth
{
    [Obsolete("Use Map property instead of MapId, and compare with MapTypes enum.")]
    public static class MapId
    {
        public const int Skeld = 0;
        public const int MiraHQ = 1;
        public const int Polus = 2;
    }
}