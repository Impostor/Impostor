using System;
using System.Linq;
using Impostor.Api;
using Impostor.Hazel;
using Impostor.Hazel.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Xunit;

namespace Impostor.Tests.Hazel
{
    public class MessageReaderTests
    {
        private ObjectPool<MessageReader> CreateReaderPool()
        {
            var services = new ServiceCollection();
            services.AddHazel();
            return services.BuildServiceProvider().GetRequiredService<ObjectPool<MessageReader>>();
        }

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

            var readerPool = CreateReaderPool();
            var reader = readerPool.Get();
            reader.Update(msg.Buffer);
            Assert.Equal(byte.MaxValue, reader.Tag);
            var message = reader.ReadMessage();
            Assert.Equal(1, message.Tag);
            Assert.Equal(Test1, message.ReadInt32());
            Assert.Equal(Test2, message.ReadInt32());
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

            var readerPool = CreateReaderPool();
            var reader = readerPool.Get();
            reader.Update(msg.Buffer);
            Assert.Equal(byte.MaxValue, reader.Tag);
            var message = reader.ReadMessage();
            Assert.Equal(1, message.Tag);
            Assert.Equal(Test1, message.ReadBoolean());
            Assert.Equal(Test2, message.ReadBoolean());
        }

        [Fact]
        public void ReadProperString()
        {
            const string Test1 = "Hello";
            var Test2 = new string(' ', 1024);
            var msg = new MessageWriter(2048);
            msg.StartMessage(1);
            msg.Write(Test1);
            msg.Write(Test2);
            msg.Write(string.Empty);
            msg.EndMessage();

            Assert.Equal(msg.Length, msg.Position);

            var readerPool = CreateReaderPool();
            var reader = readerPool.Get();
            reader.Update(msg.Buffer);
            Assert.Equal(byte.MaxValue, reader.Tag);
            var message = reader.ReadMessage();
            Assert.Equal(1, message.Tag);
            Assert.Equal(Test1, message.ReadString());
            Assert.Equal(Test2, message.ReadString());
            Assert.Equal(string.Empty, message.ReadString());
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

            var readerPool = CreateReaderPool();
            var reader = readerPool.Get();
            reader.Update(msg.Buffer);
            Assert.Equal(byte.MaxValue, reader.Tag);
            var message = reader.ReadMessage();
            Assert.Equal(1, message.Tag);
            Assert.Equal(Test1, message.ReadSingle());
        }

        [Fact]
        public void CopyMessage()
        {
            var readerPool = CreateReaderPool();

            // Create message.
            const int msgLength = 18;
            const byte Test1 = 12;
            const byte Test2 = 146;

            var msg = new MessageWriter(2048);

            msg.StartMessage(1);
            msg.StartMessage(2);
            msg.Write(Test1);
            msg.Write(Test2);
            msg.StartMessage(2);
            msg.Write(Test1);
            msg.Write(Test2);
            msg.StartMessage(2);
            msg.Write(Test1);
            msg.Write(Test2);
            msg.EndMessage();
            msg.EndMessage();
            msg.EndMessage();
            msg.EndMessage();

            // Read message.
            using var reader = readerPool.Get();

            reader.Update(msg.Buffer);

            // Read first message.
            using var messageOne = reader.ReadMessage();

            Assert.Equal(1, messageOne.Tag);
            Assert.Equal(0, messageOne.Position);
            Assert.Equal(3, messageOne.Offset);
            Assert.Equal(msgLength - 3, messageOne.Length);

            using var messageTwo = messageOne.ReadMessage();

            Assert.Equal(2, messageTwo.Tag);
            Assert.Equal(0, messageTwo.Position);
            Assert.Equal(6, messageTwo.Offset);
            Assert.Equal(msgLength - 6, messageTwo.Length);
            Assert.Equal(Test1, messageTwo.ReadByte());
            Assert.Equal(Test2, messageTwo.ReadByte());

            using var messageThree = messageTwo.ReadMessage();

            Assert.Equal(2, messageThree.Tag);
            Assert.Equal(0, messageThree.Position);
            Assert.Equal(11, messageThree.Offset);
            Assert.Equal(msgLength - 11, messageThree.Length);
            Assert.Equal(Test1, messageThree.ReadByte());
            Assert.Equal(Test2, messageThree.ReadByte());
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

            var readerPool = CreateReaderPool();
            var handleReader = readerPool.Get();
            handleReader.Update(msg.Buffer);
            var handleMessage = handleReader.ReadMessage();
            Assert.Equal(1, handleMessage.Tag);

            using var parentReader = handleMessage.Copy();

            Assert.Equal(1, parentReader.Tag);

            var reader = parentReader.ReadMessage();

            Assert.Equal(2, reader.Tag);
            Assert.Equal(Test1, reader.ReadByte());
            Assert.Equal(Test2, reader.ReadByte());
        }

        [Fact]
        public void CopyToMessage()
        {
            var expected = new byte[]
            {
                0x2A, 0x00, 0x01, 0x27, 0x00, 0x02, 0x26, 0x54,
                0x68, 0x69, 0x73, 0x20, 0x69, 0x73, 0x20, 0x61,
                0x20, 0x6C, 0x6F, 0x6E, 0x67, 0x20, 0x70, 0x61,
                0x63, 0x6B, 0x65, 0x74, 0x20, 0x74, 0x6F, 0x20,
                0x74, 0x65, 0x73, 0x74, 0x20, 0x63, 0x6F, 0x70,
                0x79, 0x69, 0x6E, 0x67, 0x2E,
            };

            var readerPool = CreateReaderPool();

            // Create packet.
            var msg = new MessageWriter(2048);
            msg.StartMessage(1);
            msg.StartMessage(2);
            msg.Write("This is a long packet to test copying.");
            msg.EndMessage();
            msg.EndMessage();

            // Create a reader.
            var reader = readerPool.Get();

            reader.Update(msg.Buffer);

            // Read the initial message.
            var message = reader.ReadMessage();

            // Copy the message to a new writer.
            var writer = new MessageWriter(2048);

            message.CopyTo(writer);

            // Compare.
            Assert.Equal(expected, writer.ToByteArray(true));
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

            var readerPool = CreateReaderPool();
            var reader = readerPool.Get();
            reader.Update(msg.Buffer);
            Assert.Equal(byte.MaxValue, reader.Tag);
            var message = reader.ReadMessage();
            Assert.Equal(1, message.Tag);
            Assert.Equal(65534, message.ReadInt32()); // Content

            var sub = message.ReadMessage();
            Assert.Equal(3, sub.Length);
            Assert.Equal(2, sub.Tag);
            Assert.Equal("HO", sub.ReadString());

            sub = message.ReadMessage();
            Assert.Equal(3, sub.Length);
            Assert.Equal(2, sub.Tag);
            Assert.Equal("NO", sub.ReadString());
        }

        [Fact]
        public void RemoveMessage()
        {
            // Create expected message.
            var messageExpected = new MessageWriter(1024);

            messageExpected.StartMessage(0);
            messageExpected.StartMessage(1);
            messageExpected.Write("HiTest1");
            messageExpected.EndMessage();
            messageExpected.StartMessage(2);
            messageExpected.Write("HiTest2");
            messageExpected.EndMessage();
            messageExpected.EndMessage();

            // Create message.
            var messageWriter = new MessageWriter(1024);

            messageWriter.StartMessage(0);
            messageWriter.StartMessage(1);
            messageWriter.Write("HiTest1");
            messageWriter.StartMessage(2);
            messageWriter.Write("RemoveMe!");
            messageWriter.EndMessage();
            messageWriter.EndMessage();
            messageWriter.StartMessage(2);
            messageWriter.Write("HiTest2");
            messageWriter.EndMessage();
            messageWriter.EndMessage();

            // Copy buffer.
            var bufferCopy = new byte[messageWriter.Length];
            Buffer.BlockCopy(messageWriter.Buffer, 0, bufferCopy, 0, bufferCopy.Length);

            var bufferCopyTwo = new byte[messageWriter.Length];
            Buffer.BlockCopy(messageWriter.Buffer, 0, bufferCopyTwo, 0, bufferCopyTwo.Length);

            // Do the magic.
            var readerPool = CreateReaderPool();
            var reader = readerPool.Get();
            reader.Update(bufferCopy);
            var inner = reader.ReadMessage();

            while (inner.Position < inner.Length)
            {
                var message = inner.ReadMessage();
                if (message.Tag == 1)
                {
                    Assert.Equal("HiTest1", message.ReadString());

                    var messageSub = message.ReadMessage();
                    if (messageSub.Tag == 2)
                    {
                        Assert.Equal("RemoveMe!", messageSub.ReadString());

                        // Remove this message.
                        inner.RemoveMessage(messageSub);
                    }
                }
                else if (message.Tag == 2)
                {
                    Assert.Equal("HiTest2", message.ReadString());
                }
                else
                {
                    Assert.True(false, "Invalid tag was read.");
                }
            }

            // Check if the magic was successful.
            Assert.Equal(messageExpected.Length, reader.Length);
            Assert.Equal(messageExpected.ToByteArray(true), reader.Buffer.Take(reader.Length).ToArray());

            // Test ownership.
            var readerTwo = readerPool.Get();

            readerTwo.Update(bufferCopyTwo);

            Assert.Throws<ImpostorProtocolException>(() => reader.RemoveMessage(readerTwo.ReadMessage()));
        }

        [Fact]
        public void GetLittleEndian()
        {
            Assert.True(MessageWriter.IsLittleEndian());
        }
    }
}
