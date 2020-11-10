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

        public static void ThrowIfEnabled(ImpostorCheatException exception)
        {
            // if (antiCheatConfig.Enabled) throw exception;
            
            // TODO: Figure out how to access the anticheat config without breaking dependency injection guidelines
            
            throw new NotImplementedException();
        }
        
        public static void ThrowIfEnabled(SerializationInfo info, StreamingContext context)
            => ThrowIfEnabled(new ImpostorCheatException(info, context));
        
        public static void ThrowIfEnabled(string? message)
            => ThrowIfEnabled(new ImpostorCheatException(message));
        
        public static void ThrowIfEnabled(string? message, Exception? innerException)
            => ThrowIfEnabled(new ImpostorCheatException(message, innerException));
    }
}