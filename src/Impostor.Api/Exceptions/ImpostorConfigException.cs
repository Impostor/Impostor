using System;

namespace Impostor.Api
{
    public class ImpostorConfigException : ImpostorException
    {
        public ImpostorConfigException()
        {
        }

        public ImpostorConfigException(string? message) : base(message)
        {
        }

        public ImpostorConfigException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
