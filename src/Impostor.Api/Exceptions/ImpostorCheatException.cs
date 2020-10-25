using System;
using System.Runtime.Serialization;

namespace Impostor.Api
{
    public class ImpostorCheatException : ImpostorException
    {
        public ImpostorCheatException()
        {
        }

        protected ImpostorCheatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ImpostorCheatException(string? message) : base(message)
        {
        }

        public ImpostorCheatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}