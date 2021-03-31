using System;
using System.Runtime.Serialization;

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

        protected ImpostorConfigException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
