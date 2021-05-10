using System;
using System.Net;
using System.Numerics;
using Impostor.Api.Games;
using Impostor.Api.Net.Inner;

namespace Impostor.Api.Net.Messages
{
    /// <summary>
    ///     Base message writer.
    /// </summary>
    public interface IMessageWriter : IDisposable
    {
        public byte[] Buffer { get; }

        public int Length { get; set; }

        public int Position { get; set; }

        public MessageType SendOption { get; }

        /// <summary>
        ///     Copies the contents of this writer into a new array.
        /// </summary>
        /// <param name="includeHeader">Whether to include message header in the array.</param>
        /// <returns>An array containing the data in the current writer.</returns>
        byte[] ToByteArray(bool includeHeader);

        /// <summary>
        ///     Writes a boolean to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(bool value);

        /// <summary>
        ///     Writes a sbyte to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(sbyte value);

        /// <summary>
        ///     Writes a byte to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(byte value);

        /// <summary>
        ///     Writes a short to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(short value);

        /// <summary>
        ///     Writes an ushort to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(ushort value);

        /// <summary>
        ///     Writes an uint to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(uint value);

        /// <summary>
        ///     Writes an int to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(int value);

        /// <summary>
        ///     Writes a float to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(float value);

        /// <summary>
        ///     Writes a string to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(string value);

        /// <summary>
        ///     Writes a <see cref="IPAddress" /> to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(IPAddress value);

        /// <summary>
        ///     Writes an packed int to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void WritePacked(int value);

        /// <summary>
        ///     Writes an packed uint to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void WritePacked(uint value);

        /// <summary>
        ///     Writes raw bytes to the message.
        /// </summary>
        /// <param name="data">Bytes to write.</param>
        void Write(ReadOnlyMemory<byte> data);

        /// <summary>
        ///     Writes a game code to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(GameCode value);

        void WriteBytesAndSize(byte[] bytes);

        void WriteBytesAndSize(byte[] bytes, int length);

        void WriteBytesAndSize(byte[] bytes, int offset, int length);

        void Write(IInnerNetObject innerNetObject);

        void Write(Vector2 vector);

        /// <summary>
        ///     Starts a new message.
        /// </summary>
        /// <param name="typeFlag">Message flag header.</param>
        void StartMessage(byte typeFlag);

        /// <summary>
        ///     Mark the end of the message.
        /// </summary>
        void EndMessage();

        /// <summary>
        ///     Clear the message writer.
        /// </summary>
        /// <param name="type">New type of the message.</param>
        void Clear(MessageType type);
    }
}
