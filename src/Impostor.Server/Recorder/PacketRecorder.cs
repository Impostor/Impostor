using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Impostor.Api.Games;
using Impostor.Api.Net.Messages;
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

        public async Task WriteConnectAsync(ClientRecorder client)
        {
            _logger.LogTrace("Writing Connect.");

            var context = _pool.Get();

            try
            {
                WriteHeader(context, RecordedPacketType.Connect);
                WriteClient(context, client, true);
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
                WriteClient(context, client, false);
                WriteLength(context);

                await WriteAsync(context.Stream);
            }
            finally
            {
                _pool.Return(context);
            }
        }

        public async Task WriteMessageAsync(ClientRecorder client, IMessageReader reader, MessageType messageType)
        {
            _logger.LogTrace("Writing Message.");

            var context = _pool.Get();

            try
            {
                WriteHeader(context, RecordedPacketType.Message);
                WriteClient(context, client, false);
                WritePacket(context, reader, messageType);
                WriteLength(context);

                await WriteAsync(context.Stream);
            }
            finally
            {
                _pool.Return(context);
            }
        }

        public async Task WriteGameCreatedAsync(ClientRecorder client, GameCode gameCode)
        {
            _logger.LogTrace("Writing GameCreated {0}.", gameCode);

            var context = _pool.Get();

            try
            {
                WriteHeader(context, RecordedPacketType.GameCreated);
                WriteClient(context, client, false);
                WriteGameCode(context, gameCode);
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

        private static void WriteClient(PacketSerializationContext context, ClientBase client, bool full)
        {
            var addressBytes = client.Connection.EndPoint.Address.GetAddressBytes();

            context.Writer.Write(client.Id);

            if (full)
            {
                context.Writer.Write((byte) addressBytes.Length);
                context.Writer.Write(addressBytes);
                context.Writer.Write((ushort) client.Connection.EndPoint.Port);
                context.Writer.Write(client.Name);
            }
        }

        private static void WritePacket(PacketSerializationContext context, IMessageReader reader, MessageType messageType)
        {
            context.Writer.Write((byte) messageType);
            context.Writer.Write((byte) reader.Tag);
            context.Writer.Write((int) reader.Buffer.Length);
            context.Writer.Write(reader.Buffer.Span);
        }

        private static void WriteGameCode(PacketSerializationContext context, in GameCode gameCode)
        {
            context.Writer.Write(gameCode.Code);
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