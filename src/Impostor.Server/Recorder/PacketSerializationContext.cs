using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Impostor.Server.Recorder
{
    public class PacketSerializationContext
    {
        private const int InitialStreamSize = 0x100;
        private const int MaximumStreamSize = 0x100000;

        private MemoryStream? _memory;
        private BinaryWriter? _writer;

        [AllowNull]
        public MemoryStream Stream
        {
            get
            {
                return _memory ??= new MemoryStream(InitialStreamSize);
            }
            private set => _memory = value;
        }

        [AllowNull]
        public BinaryWriter Writer
        {
            get
            {
                return _writer ??= new BinaryWriter(Stream, Encoding.UTF8, true);
            }
            private set => _writer = value;
        }

        public void Reset()
        {
            if (Stream.Capacity > MaximumStreamSize)
            {
                Stream = null;
                Writer = null;
            }
            else
            {
                Stream.Position = 0L;
                Stream.SetLength(0L);
            }
        }
    }
}
