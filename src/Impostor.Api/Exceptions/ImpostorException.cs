using System;
using System.Runtime.Serialization;

namespace Impostor.Api
{
    public class ImpostorException : Exception
    {
        public ImpostorException()
        {
        }

        public ImpostorException(string? message) : base(message)
        {
        }

        public ImpostorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ImpostorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
