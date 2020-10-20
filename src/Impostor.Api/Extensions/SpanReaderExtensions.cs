using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Impostor.Api.Extensions
{
    /// <summary>
    /// Priovides a StreamReader-like api throught extensions
    /// </summary>
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

            // BitConverter.Int32BitsToSingle
            // Doesn't exist in net 2.0 for some reason
            return Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(original));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(this ref ReadOnlySpan<byte> input)
        {
            return input.ReadByte() != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe float Int32BitsToSingle(int value)
        {
            return *((float*)&value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ReadOnlySpan<byte> Swap<T>(ref ReadOnlySpan<byte> input)
            where T : unmanaged
        {
            var original = input;
            input = input.Slice(sizeof(T));
            return original;
        }
    }
}