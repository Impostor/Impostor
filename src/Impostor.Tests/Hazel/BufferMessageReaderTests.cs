using Hazel;
using Impostor.Server.Net.Hazel.Messages;
using Impostor.Server.Net.Messages;
using Xunit;

namespace Impostor.Tests.Hazel
{
    public class BufferMessageReaderTests
    {
        [Fact]
        public void ReadProperInt()
        {
            const int Test1 = int.MaxValue;
            const int Test2 = int.MinValue;

            var msg = new MessageWriter(128);
            msg.StartMessage(1);
            msg.Write(Test1);
            msg.Write(Test2);
            msg.EndMessage();

            Assert.Equal(11, msg.Length);
            Assert.Equal(msg.Length, msg.Position);

            var message = new HazelMessage(new MessageReader(msg.Buffer), MessageType.Reliable);
            var reader = message.CreateReader();

            Assert.Equal(Test1, reader.ReadInt32());
            Assert.Equal(Test2, reader.ReadInt32());
        }

        [Fact]
        public void ReadProperBool()
        {
            const bool Test1 = true;
            const bool Test2 = false;

            var msg = new MessageWriter(128);
            msg.StartMessage(1);
            msg.Write(Test1);
            msg.Write(Test2);
            msg.EndMessage();

            Assert.Equal(5, msg.Length);
            Assert.Equal(msg.Length, msg.Position);

            var message = new HazelMessage(new MessageReader(msg.Buffer), MessageType.Reliable);
            var reader = message.CreateReader();

            Assert.Equal(Test1, reader.ReadBoolean());
            Assert.Equal(Test2, reader.ReadBoolean());

        }

        [Fact]
        public void ReadProperString()
        {
            const string Test1 = "Hello";
            string Test2 = new string(' ', 1024);
            var msg = new MessageWriter(2048);
            msg.StartMessage(1);
            msg.Write(Test1);
            msg.Write(Test2);
            msg.Write(string.Empty);
            msg.EndMessage();

            Assert.Equal(msg.Length, msg.Position);

            var message = new HazelMessage(new MessageReader(msg.Buffer), MessageType.Reliable);
            var reader = message.CreateReader();

            Assert.Equal(Test1, reader.ReadString());
            Assert.Equal(Test2, reader.ReadString());
            Assert.Equal(string.Empty, reader.ReadString());

        }

        [Fact]
        public void ReadProperFloat()
        {
            const float Test1 = 12.34f;

            var msg = new MessageWriter(2048);
            msg.StartMessage(1);
            msg.Write(Test1);
            msg.EndMessage();

            Assert.Equal(7, msg.Length);
            Assert.Equal(msg.Length, msg.Position);

            var message = new HazelMessage(new MessageReader(msg.Buffer), MessageType.Reliable);
            var reader = message.CreateReader();

            Assert.Equal(Test1, reader.ReadSingle());
        }

        [Fact]
        public void CopySubMessage()
        {
            const byte Test1 = 12;
            const byte Test2 = 146;

            var msg = new MessageWriter(2048);
            msg.StartMessage(1);

            msg.StartMessage(2);
            msg.Write(Test1);
            msg.Write(Test2);
            msg.EndMessage();

            msg.EndMessage();

            var message = new HazelMessage(new MessageReader(msg.Buffer), MessageType.Reliable);
            var handleMessage = message.CreateReader();
            Assert.Equal(1, handleMessage.Tag);

            var parentReader = handleMessage.Slice(handleMessage.Position);

            Assert.Equal(1, parentReader.Tag);

            var reader = parentReader.ReadMessage();

            Assert.Equal(2, reader.Tag);
            Assert.Equal(Test1, reader.ReadByte());
            Assert.Equal(Test2, reader.ReadByte());
        }

        [Fact]
        public void ReadMessageLength()
        {
            var msg = new MessageWriter(2048);
            msg.StartMessage(1);
            msg.Write(65534);
            msg.StartMessage(2);
            msg.Write("HO");
            msg.EndMessage();
            msg.StartMessage(2);
            msg.Write("NO");
            msg.EndMessage();
            msg.EndMessage();

            Assert.Equal(msg.Length, msg.Position);

            var message = new HazelMessage(new MessageReader(msg.Buffer), MessageType.Reliable);
            var reader = message.CreateReader();
            Assert.Equal(1, reader.Tag);
            Assert.Equal(65534, reader.ReadInt32()); // Content

            var sub = reader.ReadMessage();
            Assert.Equal(3, sub.Length);
            Assert.Equal(2, sub.Tag);
            Assert.Equal("HO", sub.ReadString());

            sub = reader.ReadMessage();
            Assert.Equal(3, sub.Length);
            Assert.Equal(2, sub.Tag);
            Assert.Equal("NO", sub.ReadString());
        }

        [Fact]
        public void GetLittleEndian()
        {
            Assert.True(MessageWriter.IsLittleEndian());
        }
    }
}