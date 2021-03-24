using System;
using System.Buffers.Binary;
using BenchmarkDotNet.Attributes;
using Impostor.Benchmarks.Data;
using Impostor.Benchmarks.Data.Pool;
using Impostor.Benchmarks.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace Impostor.Benchmarks.Tests
{
    public class MessageReaderBenchmark
    {
        private byte[] _data;
        private ObjectPool<MessageReader_Bytes_Pooled_Improved> _pool;

        [GlobalSetup]
        public void Setup()
        {
            var message = new MessageWriter(1024);

            message.StartMessage(1);
            message.Write((ushort)3100);
            message.Write((byte)100);
            message.Write((int)int.MaxValue);
            message.WritePacked(int.MaxValue);
            message.EndMessage();

            _data = message.ToByteArray(true);

            MessageReader_Bytes_Pooled.Return(MessageReader_Bytes_Pooled.Get(_data));

            // Services
            var services = new ServiceCollection();

            services.AddSingleton<ObjectPoolProvider>(new DefaultObjectPoolProvider());
            services.AddSingleton(serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                var policy = new MessageReader_Bytes_Pooled_ImprovedPolicy(serviceProvider);
                return provider.Create(policy);
            });

            _pool = services
                .BuildServiceProvider()
                .GetRequiredService<ObjectPool<MessageReader_Bytes_Pooled_Improved>>();
        }

        [Benchmark]
        public void Span_Run_1_000_000()
        {
            for (var i = 0; i < 1_000_000; i++)
            {
                var span = _data.AsSpan();
                var inner = span.ReadMessage();

                _ = BinaryPrimitives.ReadUInt16LittleEndian(inner);
                _ = inner[2];
                _ = BinaryPrimitives.ReadInt32LittleEndian(inner.Slice(3));
            }
        }

        // [Benchmark]
        // public void Normal_Run_1_000_000()
        // {
        //     for (var i = 0; i < 1_000_000; i++)
        //     {
        //         var reader = new MessageReader(_data);
        //         var inner = reader.ReadMessage();
        //
        //         _ = inner.ReadUInt16();
        //         _ = inner.ReadByte();
        //         _ = inner.ReadInt32();
        //         // inner.ReadPackedInt32();
        //     }
        // }

        [Benchmark]
        public void Bytes_Run_1_000_000()
        {
            for (var i = 0; i < 1_000_000; i++)
            {
                var reader = new MessageReader_Bytes(_data);
                var inner = reader.ReadMessage();

                _ = inner.ReadUInt16();
                _ = inner.ReadByte();
                _ = inner.ReadInt32();
                // inner.ReadPackedInt32();
            }
        }

        [Benchmark]
        public void Pooled_Bytes_Run_1_000_000()
        {
            for (var i = 0; i < 1_000_000; i++)
            {
                var reader = MessageReader_Bytes_Pooled.Get(_data);
                var inner = reader.ReadMessage();

                _ = inner.ReadUInt16();
                _ = inner.ReadByte();
                _ = inner.ReadInt32();
                // inner.ReadPackedInt32();

                MessageReader_Bytes_Pooled.Return(inner);
                MessageReader_Bytes_Pooled.Return(reader);
            }
        }

        [Benchmark]
        public void Improved_Pooled_Bytes_Run_1_000_000()
        {
            using (var reader = _pool.Get())
            {
                for (var i = 0; i < 1_000_000; i++)
                {
                    reader.Update(_data);

                    using (var inner = reader.ReadMessage())
                    {
                        _ = inner.ReadUInt16();
                        _ = inner.ReadByte();
                        _ = inner.ReadInt32();
                    }
                }
            }
        }
    }
}
