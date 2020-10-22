using System.IO;
using System.Text;

namespace Impostor.Server.Recorder
{
    public class PacketSerializationContext
    {
        private const int InitialStreamSize = 0x100;
        private const int MaximumStreamSize = 0x100000;

        private MemoryStream _memory;
        private BinaryWriter _writer;

        public MemoryStream Stream
        {
            get
            {
                if (_memory == null)
                {
                    _memory = new MemoryStream(InitialStreamSize);
                }

                return _memory;
            }
            private set => _memory = value;
        }

        public BinaryWriter Writer
        {
            get
            {
                if (_writer == null)
                {
                    _writer = new BinaryWriter(Stream, Encoding.UTF8, true);
                }

                return _writer;
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