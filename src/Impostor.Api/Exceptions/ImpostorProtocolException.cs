using System;
using System.Runtime.Serialization;

namespace Impostor.Api
{
    public class ImpostorProtocolException : ImpostorException
    {
        public ImpostorProtocolException()
        {
        }

        protected ImpostorProtocolException(SerializationInfo info, StreamingContext context) : base(info, context)
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
