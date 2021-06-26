using System;

namespace Impostor.Api.Innersloth
{
    [Flags]
    public enum GameKeywords : uint
    {
        All = 0,
        English = 256,
        SpanishLA = 2,
        Brazilian = 2048,
        Portuguese = 16,
        Korean = 4,
        Russian = 8,
        Dutch = 4096,
        Filipino = 64,
        French = 8192,
        German = 16384,
        Italian = 32768,
        Japanese = 512,
        SpanishEU = 1024,
        Arabic = 32,
        Polish = 128,
        Other = 1,
    }
}
