using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Server.Data;
using Impostor.Server.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;

namespace Impostor.Server.Recorder
{
    /// <summary>
    ///     Records all packets received in <see cref="ClientRecorder.HandleMessageAsync"/>.
    /// </summary>
    internal class PacketRecorder : IDisposable
    {
        private readonly ILogger<PacketRecorder> _logger;
        private readonly ObjectPool<PacketSerializationContext> _pool;
        private readonly SemaphoreSlim _writerLock;
        private readonly FileStream _writer;

        public PacketRecorder(ILogger<PacketRecorder> logger, IOptions<DebugConfig> options, ObjectPool<PacketSerializationContext> pool)
        {
            var name = $"session_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.dat";
            var path = Path.Combine(options.Value.GameRecorderPath, name);

            _logger = logger;
            _logger.LogInformation("PacketRecorder is enabled, writing packets to {0}.", path);
            _pool = pool;
            _writerLock = new SemaphoreSlim(1, 1);
            _writer = File.Open(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
        }

        public async Task WriteMessageAsync(ClientRecorder client, byte tag, ReadOnlyMemory<byte> buffer)
        {
            _logger.LogTrace("Writing Message.");

            var context = _pool.Get();

            try
            {
                WriteHeader(context, RecordedPacketType.Message);
                WriteClient(context, client);
                WritePacket(context, tag, buffer.Span);
                WriteLength(context);

                await WriteAsync(context.Stream);
            }
            finally
            {
                _pool.Return(context);
            }
        }

        public async Task WriteDisconnectAsync(ClientRecorder client)
        {
            _logger.LogTrace("Writing Disconnect.");

            var context = _pool.Get();

            try
            {
                WriteHeader(context, RecordedPacketType.Disconnect);
                WriteClient(context, client);
                WriteLength(context);

                await WriteAsync(context.Stream);
            }
            finally
            {
                _pool.Return(context);
            }
        }

        private static void WriteHeader(PacketSerializationContext context, RecordedPacketType type)
        {
            // Length placeholder.
            context.Writer.Write((int) 0);
            context.Writer.Write((byte) type);
        }

        private static void WriteClient(PacketSerializationContext context, ClientBase client)
        {
            var addressBytes = client.Connection.EndPoint.Address.GetAddressBytes();

            context.Writer.Write((byte) addressBytes.Length);
            context.Writer.Write(addressBytes);
            context.Writer.Write((ushort) client.Connection.EndPoint.Port);
        }

        private static void WritePacket(PacketSerializationContext context, byte tag, ReadOnlySpan<byte> buffer)
        {
            context.Writer.Write((byte) tag);
            context.Writer.Write((int) buffer.Length);
            context.Writer.Write(buffer);
        }

        private static void WriteLength(PacketSerializationContext context)
        {
            var length = context.Stream.Position;

            context.Stream.Position = 0;
            context.Writer.Write((int) length);
            context.Stream.Position = length;
        }

        private async Task WriteAsync(Stream data)
        {
            var hasLock = false;

            try
            {
                hasLock = await _writerLock.WaitAsync(TimeSpan.FromMinutes(1));

                if (hasLock)
                {
                    data.Position = 0;

                    await data.CopyToAsync(_writer);
                    await _writer.FlushAsync();
                }
            }
            finally
            {
                if (hasLock)
                {
                    _writerLock.Release();
                }
            }
        }

        public void Dispose()
        {
            _writer.Dispose();
            _writerLock.Dispose();
        }
    }
}