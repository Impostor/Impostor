using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.ObjectPool;
using Serilog;

namespace Impostor.Hazel.Udp
{
    /// <summary>
    ///     Represents a client's connection to a server that uses the UDP protocol.
    /// </summary>
    /// <inheritdoc/>
    public sealed class UdpClientConnection : UdpConnection
    {
        private static readonly ILogger Logger = Log.ForContext<UdpClientConnection>();

        /// <summary>
        ///     The socket we're connected via.
        /// </summary>
        private readonly UdpClient _socket;

        private readonly Timer _reliablePacketTimer;
        private readonly SemaphoreSlim _connectWaitLock;
        private readonly MemoryPool<byte> _pool;
        private readonly Channel<MessageData> _channel;
        private Task _listenTask;
        private Task _handleTask;

        /// <summary>
        ///     Creates a new UdpClientConnection.
        /// </summary>
        /// <param name="remoteEndPoint">A <see cref="NetworkEndPoint"/> to connect to.</param>
        public UdpClientConnection(IPEndPoint remoteEndPoint, ObjectPool<MessageReader> readerPool, IPMode ipMode = IPMode.IPv4) : base(null, readerPool)
        {
            EndPoint = remoteEndPoint;
            RemoteEndPoint = remoteEndPoint;
            IPMode = ipMode;

            _socket = new UdpClient
            {
                DontFragment = false
            };

            _reliablePacketTimer = new Timer(ManageReliablePacketsInternal, null, 100, Timeout.Infinite);
            _connectWaitLock = new SemaphoreSlim(1, 1);
            _pool = MemoryPool<byte>.Shared;
            _channel = Channel.CreateUnbounded<MessageData>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = true
            });
        }

        ~UdpClientConnection()
        {
            Dispose(false);
        }

        private async void ManageReliablePacketsInternal(object state)
        {
            await ManageReliablePackets();

            try
            {
                _reliablePacketTimer.Change(100, Timeout.Infinite);
            }
            catch
            {
                // ignored
            }
        }

        /// <inheritdoc />
        protected override ValueTask WriteBytesToConnection(byte[] bytes, int length)
        {
            return WriteBytesToConnectionReal(bytes, length);
        }

        private async ValueTask WriteBytesToConnectionReal(byte[] bytes, int length)
        {
            try
            {
                await _socket.SendAsync(bytes, length);
            }
            catch (NullReferenceException) { }
            catch (ObjectDisposedException)
            {
                // Already disposed and disconnected...
            }
            catch (SocketException ex)
            {
                await DisconnectInternal(HazelInternalErrors.SocketExceptionSend, "Could not send data as a SocketException occurred: " + ex.Message);
            }
        }

        /// <inheritdoc />
        public override async ValueTask ConnectAsync(byte[] bytes = null)
        {
            State = ConnectionState.Connecting;

            try
            {
                _socket.Connect(RemoteEndPoint);
            }
            catch (SocketException e)
            {
                State = ConnectionState.NotConnected;
                throw new HazelException("A SocketException occurred while binding to the port.", e);
            }

            try
            {
                _listenTask = Task.Factory.StartNew(ListenAsync, TaskCreationOptions.LongRunning);
            }
            catch (ObjectDisposedException)
            {
                // If the socket's been disposed then we can just end there but make sure we're in NotConnected state.
                // If we end up here I'm really lost...
                State = ConnectionState.NotConnected;
                return;
            }
            catch (SocketException e)
            {
                Dispose();
                throw new HazelException("A SocketException occurred while initiating a receive operation.", e);
            }

            // Write bytes to the server to tell it hi (and to punch a hole in our NAT, if present)
            // When acknowledged set the state to connected
            await SendHello(bytes, () =>
            {
                State = ConnectionState.Connected;
                InitializeKeepAliveTimer();
            });

            await _connectWaitLock.WaitAsync(TimeSpan.FromSeconds(10));
        }

        private async Task ListenAsync()
        {
            // Start packet handler.
            await StartAsync();

            // Listen.
            while (State != ConnectionState.NotConnected)
            {
                UdpReceiveResult data;

                try
                {
                    data = await _socket.ReceiveAsync();
                }
                catch (SocketException e)
                {
                    await DisconnectInternal(HazelInternalErrors.SocketExceptionReceive, "Socket exception while reading data: " + e.Message);
                    return;
                }
                catch (Exception)
                {
                    return;
                }

                if (data.Buffer.Length == 0)
                {
                    await DisconnectInternal(HazelInternalErrors.ReceivedZeroBytes, "Received 0 bytes");
                    return;
                }

                await HandleAsync(data.Buffer);
            }
        }

        private async ValueTask HandleAsync(ReadOnlyMemory<byte> memory)
        {
            // Rent memory.
            var dest = _pool.Rent(memory.Length);

            // Copy data.
            memory.CopyTo(dest.Memory);

            try
            {
                // Write to client.
                await Pipeline.Writer.WriteAsync(new MessageData(dest, memory.Length));
            }
            catch (ChannelClosedException)
            {
                // Clean up.
                dest.Dispose();
            }
        }

        protected override void SetState(ConnectionState state)
        {
            if (state == ConnectionState.Connected)
            {
                _connectWaitLock.Release();
            }
        }

        /// <summary>
        ///     Sends a disconnect message to the end point.
        ///     You may include optional disconnect data. The SendOption must be unreliable.
        /// </summary>
        protected override async ValueTask<bool> SendDisconnect(MessageWriter data = null)
        {
            lock (this)
            {
                if (_state == ConnectionState.NotConnected) return false;
                _state = ConnectionState.NotConnected;
            }

            var bytes = EmptyDisconnectBytes;
            if (data != null && data.Length > 0)
            {
                if (data.SendOption != MessageType.Unreliable)
                {
                    throw new ArgumentException("Disconnect messages can only be unreliable.");
                }

                bytes = data.ToByteArray(true);
                bytes[0] = (byte)UdpSendOption.Disconnect;
            }

            try
            {
                await _socket.SendAsync(bytes, bytes.Length, RemoteEndPoint);
            }
            catch { }

            return true;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            State = ConnectionState.NotConnected;

            try { _socket.Close(); } catch { }
            try { _socket.Dispose(); } catch { }

            _reliablePacketTimer.Dispose();
            _connectWaitLock.Dispose();

            base.Dispose(disposing);
        }
    }
}
