using System;
using System.Runtime.Serialization;

namespace Impostor.Server
{
    public class ImpostorException : Exception
    {
        public ImpostorException()
        {
        }

        protected ImpostorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ImpostorException(string? message) : base(message)
        {
        }

        public ImpostorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}