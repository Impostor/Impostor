using System;
using System.Buffers;

namespace Impostor.Hazel
{
    public readonly struct MessageData
    {
        private readonly IMemoryOwner<byte> _data;
        private readonly int _length;

        public MessageData(IMemoryOwner<byte> data, int length)
        {
            _data = data;
            _length = length;
        }

        public ReadOnlyMemory<byte> Buffer => _data.Memory.Slice(0, _length);

        public void Return()
        {
            _data.Dispose();
        }
    }
}