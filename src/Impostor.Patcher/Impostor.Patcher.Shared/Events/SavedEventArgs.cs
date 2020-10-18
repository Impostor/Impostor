using System;

namespace Impostor.Client.Shared.Events
{
    public class SavedEventArgs : EventArgs
    {
        public SavedEventArgs(string ipAddress, ushort port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        public string IpAddress { get; }
        public ushort Port { get; }
    }
}