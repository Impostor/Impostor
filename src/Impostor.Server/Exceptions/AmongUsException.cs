using System;
using System.Runtime.Serialization;

namespace Impostor.Server.Exceptions
{
    public class AmongUsException : Exception
    {
        public AmongUsException()
        {
        }

        public AmongUsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AmongUsException(string message)
            : base(message)
        {
        }

        protected AmongUsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}