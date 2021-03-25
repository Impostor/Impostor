using System;

namespace Impostor.Api.Innersloth
{
    [Flags]
    public enum GameKeywords : uint
    {
        All = 0,
        Other = 1,
        Spanish = 2,
        Korean = 4,
        Russian = 8,
        Portuguese = 16,
        Arabic = 32,
        Filipone = 64,
        Polish = 128,
        English = 256,
    }
}
