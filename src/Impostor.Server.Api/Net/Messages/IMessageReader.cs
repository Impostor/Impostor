using System;

namespace Impostor.Server.Net.Messages
{
    public interface IMessageReader
    {
        /// <summary>
        ///     Gets the tag of the message.
        /// </summary>
        byte Tag { get; }

        /// <summary>
        ///     Gets the buffer of the message.
        /// </summary>
        ReadOnlyMemory<byte> Buffer { get; }

        /// <summary>
        ///     Gets the current position of the reader.
        /// </summary>
        int Position { get; }

        /// <summary>
        ///     Gets the length of the buffer.
        /// </summary>
        int Length { get; }

        IMessageReader ReadMessage();

        bool ReadBoolean();

        sbyte ReadSByte();

        byte ReadByte();

        ushort ReadUInt16();

        short ReadInt16();

        uint ReadUInt32();

        int ReadInt32();

        float ReadSingle();

        string ReadString();

        ReadOnlyMemory<byte> ReadBytesAndSize();

        ReadOnlyMemory<byte> ReadBytes(int length);

        int ReadPackedInt32();

        uint ReadPackedUInt32();

        void CopyTo(IMessageWriter writer);

        IMessageReader Slice(int start);

        IMessageReader Slice(int start, int length);
    }
}