using System;

namespace Impostor.Patcher.Shared.Events
{
    public class SavedEventArgs : EventArgs
    {
        public SavedEventArgs(string ipAddress, ushort port, string host)
        {
            IpAddress = ipAddress;
            Port = port;
            Host = host;
        }

        public string IpAddress { get; }
        public ushort Port { get; }
        public string Host { get; }
    }
}
