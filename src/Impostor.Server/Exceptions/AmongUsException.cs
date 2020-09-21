using System;
using System.Runtime.Serialization;

namespace Impostor.Server.Exceptions
{
    public class AmongUsException : Exception
    {
        public AmongUsException()
        {
        }

        protected AmongUsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public AmongUsException(string message) : base(message)
        {
        }

        public AmongUsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}