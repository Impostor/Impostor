using System;

namespace Impostor.Server.Net
{
    public interface IMessageReader
    {
        int Position { get; }
        
        ReadOnlyMemory<byte> Buffer { get; }
        
        byte Tag { get; }
        
        int Length { get; }
        
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
    }
}