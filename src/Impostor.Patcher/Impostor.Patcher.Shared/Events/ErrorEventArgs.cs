using System;

namespace Impostor.Client.Shared.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}