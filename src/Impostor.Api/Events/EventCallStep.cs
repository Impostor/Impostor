using System;

namespace Impostor.Api.Events
{
    [Flags]
    public enum EventCallStep
    {
        Post = 0b01,
        Pre = 0b10,
        All = Post | Pre,
    }
}
