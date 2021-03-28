using System;

namespace Impostor.Api
{
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
    }
}
