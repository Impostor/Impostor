using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Impostor.Api
{
    /// <summary>
    ///     Priovides a StreamReader-like api throught extensions.
    /// </summary>
    public static class SpanReaderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(this ref ReadOnlySpan<byte> input)
        {
            var original = Advance<byte>(ref input);
            return original[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt32(this ref ReadOnlySpan<byte> input)
        {
            var original = Advance<int>(ref input);
            return BinaryPrimitives.ReadInt32LittleEndian(original);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt32(this ref ReadOnlySpan<byte> input)
        {
            var original = Advance<uint>(ref input);
            return BinaryPrimitives.ReadUInt32LittleEndian(original);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadSingle(this ref ReadOnlySpan<byte> input)
        {
            var original = Advance<float>(ref input);

            return BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(original));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBoolean(this ref ReadOnlySpan<byte> input)
        {
            return input.ReadByte() != 0;
        }

        /// <summary>
        ///     Advances the position of <paramref name="input" /> by the size of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">Type that will be read.</typeparam>
        /// <param name="input">input "stream"/span.</param>
        /// <returns>The original input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ReadOnlySpan<byte> Advance<T>(ref ReadOnlySpan<byte> input)
            where T : unmanaged
        {
            var original = input;
            input = input.Slice(sizeof(T));
            return original;
        }
    }
}
