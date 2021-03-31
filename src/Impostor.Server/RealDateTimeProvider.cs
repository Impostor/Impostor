using System;
using Impostor.Api;

namespace Impostor.Server
{
    public class RealDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
