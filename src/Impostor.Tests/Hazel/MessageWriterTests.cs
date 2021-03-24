using System;
using System.Linq;
using Impostor.Hazel;
using Xunit;

namespace Impostor.Tests.Hazel
{
    public class MessageWriterTests
    {
        [Fact]
        public void ReadOnlyMemoryWriteWorksTheSameAsArray()
        {
            var oldVer = new MessageWriter(1024);
            var newVer = new MessageWriter(1024);

            var data = Enumerable.Repeat
            (
                Enumerable.Range(0, byte.MaxValue)
                    .Select(x => (byte)x),
                2
            ).SelectMany(x => x).ToArray();

            WriteSomeData(oldVer);
            WriteSomeData(newVer);

            oldVer.Write(data);
            newVer.Write(data.AsMemory());

            Assert.True(oldVer.Buffer.AsSpan().SequenceEqual(newVer.Buffer.AsSpan()));

            static void WriteSomeData(MessageWriter oldVer)
            {
                oldVer.WritePacked(99);
                oldVer.WritePacked(101);
            }
        }
    }
}
