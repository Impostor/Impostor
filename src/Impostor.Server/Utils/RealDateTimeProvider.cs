using System;
using Impostor.Api.Utils;

namespace Impostor.Server.Utils
{
    public class RealDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
