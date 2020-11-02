using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;
using Serilog;

namespace Impostor.Hazel.Udp
{
    /// <summary>
    ///     Listens for new UDP connections and creates UdpConnections for them.
    /// </summary>
    /// <inheritdoc />
    public class UdpConnectionListener : NetworkConnectionListener
    {
        private static readonly ILogger Logger = Log.ForContext<UdpConnectionListener>();

        /// <summary>
        /// A callback for early connection rejection. 
        /// * Return false to reject connection.
        /// * A null response is ok, we just won't send anything.
        /// </summary>
        public AcceptConnectionCheck AcceptConnection;
        public delegate bool AcceptConnectionCheck(IPEndPoint endPoint, byte[] input, out byte[] response);

        private readonly UdpClient _socket;
        private readonly ObjectPool<MessageReader> _readerPool;
        private readonly MemoryPool<byte> _pool;
        private readonly Timer _reliablePacketTimer;
        private readonly ConcurrentDictionary<EndPoint, UdpServerConnection> _allConnections;
        private readonly CancellationTokenSource _stoppingCts;
        private readonly UdpConnectionRateLimit _connectionRateLimit;
        private Task _executingTask;

        /// <summary>
        ///     Creates a new UdpConnectionListener for the given <see cref="IPAddress"/>, port and <see cref="IPMode"/>.
        /// </summary>
        /// <param name="endPoint">The endpoint to listen on.</param>
        /// <param name="ipMode"></param>
        public UdpConnectionListener(IPEndPoint endPoint, ObjectPool<MessageReader> readerPool, IPMode ipMode = IPMode.IPv4)
        {
            EndPoint = endPoint;
            IPMode = ipMode;

            _readerPool = readerPool;
            _pool = MemoryPool<byte>.Shared;
            _socket = new UdpClient(endPoint)
            {
                DontFragment = false
            };

            _reliablePacketTimer = new Timer(ManageReliablePackets, null, 100, Timeout.Infinite);

            _allConnections = new ConcurrentDictionary<EndPoint, UdpServerConnection>();

            _stoppingCts = new CancellationTokenSource();
            _stoppingCts.Token.Register(() =>
            {
                _socket.Dispose();
            });

            _connectionRateLimit = new UdpConnectionRateLimit();
        }

        public int ConnectionCount => this._allConnections.Count;
        
        private async void ManageReliablePackets(object state)
        {
            foreach (var kvp in _allConnections)
            {
                var sock = kvp.Value;
                await sock.ManageReliablePackets();
            }

            try
            {
                this._reliablePacketTimer.Change(100, Timeout.Infinite);
            }
            catch { }
        }

        /// <inheritdoc />
        public override Task StartAsync()
        {
            // Store the task we're executing
            _executingTask = Task.Factory.StartNew(ListenAsync, TaskCreationOptions.LongRunning);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        private async Task StopAsync()
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the timeout triggers
                await Task.WhenAny(_executingTask, Task.Delay(TimeSpan.FromSeconds(5)));
            }
        }

        /// <summary>
        ///     Instructs the listener to begin listening.
        /// </summary>
        private async Task ListenAsync()
        {
            try
            {
                while (!_stoppingCts.IsCancellationRequested)
                {
                    UdpReceiveResult data;

                    try
                    {
                        data = await _socket.ReceiveAsync();

                        if (data.Buffer.Length == 0)
                        {
                            Logger.Fatal("Hazel read 0 bytes from UDP server socket.");
                            continue;
                        }
                    }
                    catch (SocketException)
                    {
                        // Client no longer reachable, pretend it didn't happen
                        continue;
                    }
                    catch (ObjectDisposedException)
                    {
                        // Socket was disposed, don't care.
                        return;
                    }

                    // Get client from active clients
                    if (!_allConnections.TryGetValue(data.RemoteEndPoint, out var client))
                    {
                        // Check for malformed connection attempts
                        if (data.Buffer[0] != (byte)UdpSendOption.Hello)
                        {
                            continue;
                        }

                        // Check rateLimit.
                        if (!_connectionRateLimit.IsAllowed(data.RemoteEndPoint.Address))
                        {
                            Logger.Warning("Ratelimited connection attempt from {0}.", data.RemoteEndPoint);
                            continue;
                        }

                        // Create new client
                        client = new UdpServerConnection(this, data.RemoteEndPoint, IPMode, _readerPool);

                        // Store the client
                        if (!_allConnections.TryAdd(data.RemoteEndPoint, client))
                        {
                            throw new HazelException("Failed to add a connection. This should never happen.");
                        }

                        // Activate the reader loop of the client
                        await client.StartAsync();
                    }

                    // Write to client.
                    await client.Pipeline.Writer.WriteAsync(data.Buffer);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Listen loop error");
            }
        }

#if DEBUG
        public int TestDropRate = -1;
        private int dropCounter = 0;
#endif

        /// <summary>
        ///     Sends data from the listener socket.
        /// </summary>
        /// <param name="bytes">The bytes to send.</param>
        /// <param name="endPoint">The endpoint to send to.</param>
        internal async ValueTask SendData(byte[] bytes, int length, IPEndPoint endPoint)
        {
            if (length > bytes.Length) return;

#if DEBUG
            if (TestDropRate > 0)
            {
                if (Interlocked.Increment(ref dropCounter) % TestDropRate == 0)
                {
                    return;
                }
            }
#endif

            try
            {
                await _socket.SendAsync(bytes, length, endPoint);
            }
            catch (SocketException e)
            {
                Logger.Error(e, "Could not send data as a SocketException occurred");
            }
            catch (ObjectDisposedException)
            {
                //Keep alive timer probably ran, ignore
                return;
            }
        }

        /// <summary>
        ///     Sends data from the listener socket.
        /// </summary>
        /// <param name="bytes">The bytes to send.</param>
        /// <param name="length"></param>
        /// <param name="endPoint">The endpoint to send to.</param>
        internal void SendDataSync(byte[] bytes, int length, IPEndPoint endPoint)
        {
            try
            {
                _socket.Send(bytes, length, endPoint);
            }
            catch (SocketException e)
            {
                Logger.Error(e, "Could not send data sync as a SocketException occurred");
            }
        }

        /// <summary>
        ///     Removes a virtual connection from the list.
        /// </summary>
        /// <param name="endPoint">The endpoint of the virtual connection.</param>
        internal void RemoveConnectionTo(EndPoint endPoint)
        {
            this._allConnections.TryRemove(endPoint, out var conn);
        }

        /// <inheritdoc />
        public override async ValueTask DisposeAsync()
        {
            foreach (var kvp in _allConnections)
            {
                kvp.Value.Dispose();
            }

            await StopAsync();

            await _reliablePacketTimer.DisposeAsync();

            _connectionRateLimit.Dispose();

            await base.DisposeAsync();
        }
    }
}
