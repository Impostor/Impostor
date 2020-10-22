using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Impostor.Benchmarks.Logic.GameOptionsDataLogic
{
    public static class SpanWriterExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteByte(this ref Span<byte> input, byte value)
        {
            var original = Swap<byte>(ref input);
            original[0] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe Span<byte> Swap<T>(ref Span<byte> input) where T : unmanaged
        {
            var original = input;
            var slized = input.Slice(sizeof(T));
            input = slized;
            return original;
        }
    }

    public static class SpanReaderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(this ref ReadOnlySpan<byte> input)
        {
            var original = Swap<byte>(ref input);
            return original[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(this ref ReadOnlySpan<byte> input)
        {
            var original = Swap<int>(ref input);
            return BinaryPrimitives.ReadInt32LittleEndian(original);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(this ref ReadOnlySpan<byte> input)
        {
            var original = Swap<uint>(ref input);
            return BinaryPrimitives.ReadUInt32LittleEndian(original);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(this ref ReadOnlySpan<byte> input)
        {
            var original = Swap<float>(ref input);
            return BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(original));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(this ref ReadOnlySpan<byte> input)
        {
            return input.ReadByte() != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ReadOnlySpan<byte> Swap<T>(ref ReadOnlySpan<byte> input) where T : unmanaged
        {
            var original = input;
            var slized = input.Slice(sizeof(T));
            input = slized;
            return original;
        }
    }
}