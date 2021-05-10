using System;

namespace Impostor.Api.Utils
{
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
    }
}
