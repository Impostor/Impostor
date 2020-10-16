using System;

namespace Impostor.Server.GameData
{
    [Flags]
    public enum SpawnFlags : byte
    {
        None = 0,
        IsClientCharacter = 1,
    }
}