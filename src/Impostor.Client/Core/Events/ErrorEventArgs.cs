using System;

namespace Impostor.Client.Core.Events
{
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(string message, bool openLocalLow)
        {
            Message = message;
            OpenLocalLow = openLocalLow;
        }
        
        public string Message { get; }

        public bool OpenLocalLow { get; }
    }
}