using System;

namespace Impostor.Api;

public class ImpostorException : Exception
{
    protected ImpostorException()
    {
    }

    public ImpostorException(string? message) : base(message)
    {
    }

    protected ImpostorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
