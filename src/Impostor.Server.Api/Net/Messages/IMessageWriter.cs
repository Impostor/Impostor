using System;
using System.Net;
using Impostor.Server.Games;

namespace Impostor.Server.Net.Messages
{
    /// <summary>
    ///     Base message writer.
    /// </summary>
    public interface IMessageWriter : IDisposable
    {
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
        ///     Writes a <see cref="IPAddress"/> to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void Write(IPAddress value);

        /// <summary>
        ///     Writes an packed int to the message.
        /// </summary>
        /// <param name="value">Value to write.</param>
        void WritePacked(int value);

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