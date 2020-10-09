using System;
using System.Net;

namespace Impostor.Server.Net
{
    public interface IMessageWriter : IDisposable
    {
        void Write(bool value);

        void Write(sbyte value);

        void Write(byte value);

        void Write(short value);

        void Write(ushort value);

        void Write(uint value);

        void Write(int value);

        void Write(float value);

        void Write(string value);
        
        void Write(IPAddress ipAddress);
        
        void WritePacked(int value);
        
        void Write(ReadOnlyMemory<byte> data);
        
        void StartMessage(byte typeFlag);

        void Write(GameCode code);
        
        void EndMessage();

        void Clear(MessageType type);
    }
}