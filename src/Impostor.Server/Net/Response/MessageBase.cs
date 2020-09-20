using System;
using Hazel;

namespace Impostor.Server.Net.Response
{
    public abstract class MessageBase : IDisposable
    {
        private readonly MessageWriter _writer;
        private readonly MessageFlag _flag;

        protected MessageBase(SendOption option, MessageFlag flag)
        {
            _writer = MessageWriter.Get(option);
            _flag = flag;
        }

        public MessageWriter Write()
        {
            _writer.StartMessage((byte) _flag);
            WriteMessage(_writer);
            _writer.EndMessage();
            return _writer;
        }

        protected abstract void WriteMessage(MessageWriter writer);

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}