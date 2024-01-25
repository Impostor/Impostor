using System;

namespace Impostor.Api
{
    public class ImpostorProtocolException : ImpostorException
    {
        public ImpostorProtocolException()
        {
        }

        public ImpostorProtocolException(string? message) : base(message)
        {
        }

        public ImpostorProtocolException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
