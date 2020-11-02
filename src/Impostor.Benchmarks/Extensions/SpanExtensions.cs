using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Impostor.Benchmarks.Extensions
{
    public static class SpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Span<byte> ReadMessage(this Span<byte> input)
        {
            var length = BinaryPrimitives.ReadUInt16LittleEndian(input);
            var tag = input[2];

            return input.Slice(3, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadUInt16(this ref ReadOnlySpan<byte> input)
        {
            return BinaryPrimitives.ReadUInt16LittleEndian(input);
        }
    }
}
