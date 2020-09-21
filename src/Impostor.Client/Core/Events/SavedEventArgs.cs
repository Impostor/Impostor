using System;

namespace Impostor.Client.Core.Events
{
    public class SavedEventArgs : EventArgs
    {
        public SavedEventArgs(string ipAddress)
        {
            IpAddress = ipAddress;
        }
        
        public string IpAddress { get; }
    }
}