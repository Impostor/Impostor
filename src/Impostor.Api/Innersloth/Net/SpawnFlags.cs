using System;

namespace Impostor.Api.Innersloth.Net
{
    [Flags]
    public enum SpawnFlags : byte
    {
        None = 0,
        IsClientCharacter = 1,
    }
}