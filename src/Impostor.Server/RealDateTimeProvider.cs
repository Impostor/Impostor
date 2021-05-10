using System;
using Impostor.Api.Utils;

namespace Impostor.Server
{
    public class RealDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
