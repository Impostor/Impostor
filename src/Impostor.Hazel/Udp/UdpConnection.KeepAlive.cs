using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Impostor.Hazel.Udp
{
    partial class UdpConnection
    {

        /// <summary>
        ///     Class to hold packet data
        /// </summary>
        public class PingPacket : IRecyclable
        {
            private static readonly ObjectPoolCustom<PingPacket> PacketPool = new ObjectPoolCustom<PingPacket>(() => new PingPacket());

            public readonly Stopwatch Stopwatch = new Stopwatch();

            internal static PingPacket GetObject()
            {
                return PacketPool.GetObject();
            }

            public void Recycle()
            {
                Stopwatch.Stop();
                PacketPool.PutObject(this);
            }
        }

        internal ConcurrentDictionary<ushort, PingPacket> activePingPackets = new ConcurrentDictionary<ushort, PingPacket>();

        /// <summary>
        ///     The interval from data being received or transmitted to a keepalive packet being sent in milliseconds.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Keepalive packets serve to close connections when an endpoint abruptly disconnects and to ensure than any
        ///         NAT devices do not close their translation for our argument. By ensuring there is regular contact the
        ///         connection can detect and prevent these issues.
        ///     </para>
        ///     <para>
        ///         The default value is 10 seconds, set to System.Threading.Timeout.Infinite to disable keepalive packets.
        ///     </para>
        /// </remarks>
        public int KeepAliveInterval
        {
            get
            {
                return keepAliveInterval;
            }

            set
            {
                keepAliveInterval = value;
                ResetKeepAliveTimer();
            }
        }
        private int keepAliveInterval = 1500;

        public int MissingPingsUntilDisconnect { get; set; } = 6;
        private volatile int pingsSinceAck = 0;

        /// <summary>
        ///     The timer creating keepalive pulses.
        /// </summary>
        private Timer keepAliveTimer;

        /// <summary>
        ///     Starts the keepalive timer.
        /// </summary>
        protected void InitializeKeepAliveTimer()
        {
            keepAliveTimer = new Timer(
                HandleKeepAlive,
                null,
                keepAliveInterval,
                keepAliveInterval
            );
        }

        private async void HandleKeepAlive(object state)
        {
            if (this.State != ConnectionState.Connected) return;

            if (this.pingsSinceAck >= this.MissingPingsUntilDisconnect)
            {
                this.DisposeKeepAliveTimer();
                await this.DisconnectInternal(HazelInternalErrors.PingsWithoutResponse, $"Sent {this.pingsSinceAck} pings that remote has not responded to.");
                return;
            }

            try
            {
                Interlocked.Increment(ref pingsSinceAck);
                await SendPing();
            }
            catch
            {
            }
        }

        // Pings are special, quasi-reliable packets. 
        // We send them to trigger responses that validate our connection is alive
        // An unacked ping should never be the sole cause of a disconnect.
        // Rather, the responses will reset our pingsSinceAck, enough unacked 
        // pings should cause a disconnect.
        private async ValueTask SendPing()
        {
            ushort id = (ushort)Interlocked.Increment(ref lastIDAllocated);

            byte[] bytes = new byte[3];
            bytes[0] = (byte)UdpSendOption.Ping;
            bytes[1] = (byte)(id >> 8);
            bytes[2] = (byte)id;

            PingPacket pkt;
            if (!this.activePingPackets.TryGetValue(id, out pkt))
            {
                pkt = PingPacket.GetObject();
                if (!this.activePingPackets.TryAdd(id, pkt))
                {
                    throw new Exception("This shouldn't be possible");
                }
            }

            pkt.Stopwatch.Restart();

            await WriteBytesToConnection(bytes, bytes.Length);

            Statistics.LogReliableSend(0, bytes.Length);
        }

        /// <summary>
        ///     Resets the keepalive timer to zero.
        /// </summary>
        private void ResetKeepAliveTimer()
        {
            try
            {
                keepAliveTimer.Change(keepAliveInterval, keepAliveInterval);
            }
            catch { }
        }

        /// <summary>
        ///     Disposes of the keep alive timer.
        /// </summary>
        private void DisposeKeepAliveTimer()
        {
            if (this.keepAliveTimer != null)
            {
                this.keepAliveTimer.Dispose();
            }

            foreach (var kvp in activePingPackets)
            {
                if (this.activePingPackets.TryRemove(kvp.Key, out var pkt))
                {
                    pkt.Recycle();
                }
            }
        }
    }
}