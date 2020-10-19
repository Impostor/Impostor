using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Impostor.Hazel.Udp
{
    ///
    public class UdpBroadcaster : IDisposable
    {
        private Socket socket;
        private byte[] data;
        private EndPoint endpoint;
        private Action<string> logger;

        ///
        public UdpBroadcaster(int port, Action<string> logger = null)
        {
            this.logger = logger;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.socket.EnableBroadcast = true;
            this.socket.MulticastLoopback = false;
            this.endpoint = new IPEndPoint(IPAddress.Broadcast, port);
        }

        ///
        public void SetData(string data)
        {
            int len = UTF8Encoding.UTF8.GetByteCount(data);
            this.data = new byte[len + 2];
            this.data[0] = 4;
            this.data[1] = 2;

            UTF8Encoding.UTF8.GetBytes(data, 0, data.Length, this.data, 2);
        }

        ///
        public void Broadcast()
        {
            if (this.data == null)
            {
                return;
            }

            try
            {
                this.socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, this.endpoint, this.FinishSendTo, null);
            }
            catch (Exception e)
            {
                this.logger?.Invoke("BroadcastListener: " + e);
            }
        }

        private void FinishSendTo(IAsyncResult evt)
        {
            try
            {
                this.socket.EndSendTo(evt);
            }
            catch (Exception e)
            {
                this.logger?.Invoke("BroadcastListener: " + e);
            }
        }

        ///
        public void Dispose()
        {
            if (this.socket != null)
            {
                try { this.socket.Shutdown(SocketShutdown.Both); } catch { }
                try { this.socket.Close(); } catch { }
                try { this.socket.Dispose(); } catch { }
                this.socket = null;
            }
        }
    }
}