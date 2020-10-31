using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Impostor.Api.Net.Messages;
using Microsoft.Extensions.ObjectPool;
using Serilog;

namespace Impostor.Hazel.Udp
{
    /// <summary>
    ///     Represents a connection that uses the UDP protocol.
    /// </summary>
    /// <inheritdoc />
    public abstract partial class UdpConnection : NetworkConnection
    {
        protected static readonly byte[] EmptyDisconnectBytes = { (byte)UdpSendOption.Disconnect };

        private static readonly ILogger Logger = Log.ForContext<UdpConnection>();
        private readonly ConnectionListener _listener;
        private readonly ObjectPool<MessageReader> _readerPool;
        private readonly CancellationTokenSource _stoppingCts;

        private bool _isDisposing;
        private bool _isFirst = true;
        private Task _executingTask;

        protected UdpConnection(ConnectionListener listener, ObjectPool<MessageReader> readerPool)
        {
            _listener = listener;
            _readerPool = readerPool;
            _stoppingCts = new CancellationTokenSource();

            Pipeline = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = true
            });
        }

        internal Channel<byte[]> Pipeline { get; }

        public Task StartAsync()
        {
            // Store the task we're executing
            _executingTask = Task.Factory.StartNew(ReadAsync, TaskCreationOptions.LongRunning);

            // If the task is completed then return it, this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public void Stop()
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            // Signal cancellation to methods.
            _stoppingCts.Cancel();

            try
            {
                // Cancel reader.
                Pipeline.Writer.Complete();
            }
            catch (ChannelClosedException)
            {
                // Already done.
            }

            // Remove references.
            if (!_isDisposing)
            {
                Dispose(true);
            }
        }

        private async Task ReadAsync()
        {
            var reader = new MessageReader(_readerPool);

            while (!_stoppingCts.IsCancellationRequested)
            {
                var result = await Pipeline.Reader.ReadAsync(_stoppingCts.Token);

                try
                {
                    reader.Update(result);

                    await HandleReceive(reader);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Exception during ReadAsync");
                    Dispose(true);
                    break;
                }
            }
        }

        /// <summary>
        ///     Writes the given bytes to the connection.
        /// </summary>
        /// <param name="bytes">The bytes to write.</param>
        /// <param name="length"></param>
        protected abstract ValueTask WriteBytesToConnection(byte[] bytes, int length);

        /// <inheritdoc/>
        public override async ValueTask SendAsync(IMessageWriter msg)
        {
            if (this._state != ConnectionState.Connected)
                throw new InvalidOperationException("Could not send data as this Connection is not connected. Did you disconnect?");

            byte[] buffer = new byte[msg.Length];
            Buffer.BlockCopy(msg.Buffer, 0, buffer, 0, msg.Length);

            switch (msg.SendOption)
            {
                case MessageType.Reliable:
                    ResetKeepAliveTimer();

                    AttachReliableID(buffer, 1, buffer.Length);
                    await WriteBytesToConnection(buffer, buffer.Length);
                    Statistics.LogReliableSend(buffer.Length - 3, buffer.Length);
                    break;

                default:
                    await WriteBytesToConnection(buffer, buffer.Length);
                    Statistics.LogUnreliableSend(buffer.Length - 1, buffer.Length);
                    break;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        ///     <include file="DocInclude/common.xml" path="docs/item[@name='Connection_SendBytes_General']/*" />
        ///     <para>
        ///         Udp connections can currently send messages using <see cref="SendOption.None"/> and
        ///         <see cref="SendOption.Reliable"/>. Fragmented messages are not currently supported and will default to
        ///         <see cref="SendOption.None"/> until implemented.
        ///     </para>
        /// </remarks>
        public override async ValueTask SendBytes(byte[] bytes, MessageType sendOption = MessageType.Unreliable)
        {
            //Add header information and send
            await HandleSend(bytes, (byte)sendOption);
        }
        
        /// <summary>
        ///     Handles the reliable/fragmented sending from this connection.
        /// </summary>
        /// <param name="data">The data being sent.</param>
        /// <param name="sendOption">The <see cref="SendOption"/> specified as its byte value.</param>
        /// <param name="ackCallback">The callback to invoke when this packet is acknowledged.</param>
        /// <returns>The bytes that should actually be sent.</returns>
        protected async ValueTask HandleSend(byte[] data, byte sendOption, Action ackCallback = null)
        {
            switch (sendOption)
            {
                case (byte)UdpSendOption.Ping:
                case (byte)MessageType.Reliable:
                case (byte)UdpSendOption.Hello:
                    await ReliableSend(sendOption, data, ackCallback);
                    break;
                                    
                //Treat all else as unreliable
                default:
                    await UnreliableSend(sendOption, data);
                    break;
            }
        }

        /// <summary>
        ///     Handles the receiving of data.
        /// </summary>
        /// <param name="message">The buffer containing the bytes received.</param>
        protected async ValueTask HandleReceive(MessageReader message)
        {
            // Check if the first message received is the hello packet.
            if (_isFirst)
            {
                _isFirst = false;

                // Slice 4 bytes to get handshake data.
                if (_listener != null)
                {
                    using (var handshake = message.Copy(4))
                    {
                        await _listener.InvokeNewConnection(handshake, this);
                    }
                }
            }

            switch (message.Buffer[0])
            {
                //Handle reliable receives
                case (byte)MessageType.Reliable:
                    await ReliableMessageReceive(message);
                    break;

                //Handle acknowledgments
                case (byte)UdpSendOption.Acknowledgement:
                    AcknowledgementMessageReceive(message.Buffer);
                    break;

                //We need to acknowledge hello and ping messages but dont want to invoke any events!
                case (byte)UdpSendOption.Ping:
                    await ProcessReliableReceive(message.Buffer, 1);
                    Statistics.LogHelloReceive(message.Length);
                    break;
                case (byte)UdpSendOption.Hello:
                    await ProcessReliableReceive(message.Buffer, 1);
                    Statistics.LogHelloReceive(message.Length);
                    break;

                case (byte)UdpSendOption.Disconnect:
                    using (var reader = message.Copy(1))
                    {
                        await DisconnectRemote("The remote sent a disconnect request", reader);
                    }
                    break;
                    
                //Treat everything else as unreliable
                default:
                    using (var reader = message.Copy(1))
                    {
                        await InvokeDataReceived(reader, MessageType.Unreliable);
                    }
                    Statistics.LogUnreliableReceive(message.Length - 1, message.Length);
                    break;
            }
        }

        /// <summary>
        ///     Sends bytes using the unreliable UDP protocol.
        /// </summary>
        /// <param name="sendOption">The SendOption to attach.</param>
        /// <param name="data">The data.</param>
        ValueTask UnreliableSend(byte sendOption, byte[] data)
        {
            return UnreliableSend(sendOption, data, 0, data.Length);
        }

        /// <summary>
        ///     Sends bytes using the unreliable UDP protocol.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="sendOption">The SendOption to attach.</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        async ValueTask UnreliableSend(byte sendOption, byte[] data, int offset, int length)
        {
            byte[] bytes = new byte[length + 1];

            //Add message type
            bytes[0] = sendOption;

            //Copy data into new array
            Buffer.BlockCopy(data, offset, bytes, bytes.Length - length, length);

            //Write to connection
            await WriteBytesToConnection(bytes, bytes.Length);

            Statistics.LogUnreliableSend(length, bytes.Length);
        }

        /// <summary>
        ///     Sends a hello packet to the remote endpoint.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="acknowledgeCallback">The callback to invoke when the hello packet is acknowledged.</param>
        protected ValueTask SendHello(byte[] bytes, Action acknowledgeCallback)
        {
            //First byte of handshake is version indicator so add data after
            byte[] actualBytes;
            if (bytes == null)
            {
                actualBytes = new byte[1];
            }
            else
            {
                actualBytes = new byte[bytes.Length + 1];
                Buffer.BlockCopy(bytes, 0, actualBytes, 1, bytes.Length);
            }

            return HandleSend(actualBytes, (byte)UdpSendOption.Hello, acknowledgeCallback);
        }
                
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isDisposing = true;

                Stop();
                DisposeKeepAliveTimer();
                DisposeReliablePackets();
            }

            base.Dispose(disposing);
        }
    }
}
