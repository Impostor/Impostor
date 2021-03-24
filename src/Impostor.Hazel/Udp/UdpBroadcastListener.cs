using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Impostor.Hazel.Udp
{
    public class BroadcastPacket
    {
        public string Data;
        public DateTime ReceiveTime;
        public IPEndPoint Sender;

        public BroadcastPacket(string data, IPEndPoint sender)
        {
            this.Data = data;
            this.Sender = sender;
            this.ReceiveTime = DateTime.Now;
        }

        public string GetAddress()
        {
            return this.Sender.Address.ToString();
        }
    }

    public class UdpBroadcastListener : IDisposable
    {
        private Socket socket;
        private EndPoint endpoint;
        private Action<string> logger;

        private byte[] buffer = new byte[1024];

        private List<BroadcastPacket> packets = new List<BroadcastPacket>();

        public bool Running { get; private set; }

        ///
        public UdpBroadcastListener(int port, Action<string> logger = null)
        {
            this.logger = logger;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.socket.EnableBroadcast = true;
            this.socket.MulticastLoopback = false;
            this.endpoint = new IPEndPoint(IPAddress.Any, port);
            this.socket.Bind(this.endpoint);
        }

        ///
        public void StartListen()
        {
            if (this.Running) return;
            this.Running = true;

            try
            {
                EndPoint endpt = new IPEndPoint(IPAddress.Any, 0);
                this.socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endpt, this.HandleData, null);
            }
            catch (NullReferenceException) { }
            catch (Exception e)
            {
                this.logger?.Invoke("BroadcastListener: " + e);
                this.Dispose();
            }
        }

        private void HandleData(IAsyncResult result)
        {
            this.Running = false;

            int numBytes;
            EndPoint endpt = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                numBytes = this.socket.EndReceiveFrom(result, ref endpt);
            }
            catch (NullReferenceException)
            {
                // Already disposed
                return;
            }
            catch (Exception e)
            {
                this.logger?.Invoke("BroadcastListener: " + e);
                this.Dispose();
                return;
            }

            if (numBytes < 3 
                || buffer[0] != 4 || buffer[1] != 2)
            {
                this.StartListen();
                return;
            }

            IPEndPoint ipEnd = (IPEndPoint)endpt;
            string data = UTF8Encoding.UTF8.GetString(buffer, 2, numBytes - 2);
            int dataHash = data.GetHashCode();

            lock (packets)
            {
                bool found = false;
                for (int i = 0; i < this.packets.Count; ++i)
                {
                    var pkt = this.packets[i];
                    if (pkt == null || pkt.Data == null)
                    {
                        this.packets.RemoveAt(i);
                        i--;
                        continue;
                    }

                    if (pkt.Data.GetHashCode() == dataHash
                        && pkt.Sender.Equals(ipEnd))
                    {
                        this.packets[i].ReceiveTime = DateTime.Now;
                        break;
                    }
                }

                if (!found)
                {
                    this.packets.Add(new BroadcastPacket(data, ipEnd));
                }
            }

            this.StartListen();
        }

        ///
        public BroadcastPacket[] GetPackets()
        {
            lock (this.packets)
            {
                var output = this.packets.ToArray();
                this.packets.Clear();
                return output;
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