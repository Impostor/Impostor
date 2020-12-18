using System.Drawing;
using Impostor.Api.Innersloth.Text;
using Xunit;

namespace Impostor.Tests
{
    public class TextTests
    {
        private Text CreateTestText()
        {
            return new Text("white")
                .Append(" ")
                .Append("red", Color.Red)
                .Append(" ")
                .Append("link", link: "https://example.com");
        }

        [Fact]
        public void Serialize()
        {
            var text = CreateTestText();

            Assert.Equal("white [FF0000FF]red[] [https://example.com]link[]", text.ToString());
            Assert.Equal("white red link", text.ToRawString());
        }

        [Fact]
        public void Deserialize()
        {
            var text = CreateTestText();
            var parsed = Text.Parse("white [FF0000FF]red[] [https://example.com]link[]");

            Assert.Equal(text.ToString(), parsed.ToString());
        }

        [Fact]
        public void Roundtrip()
        {
            var text = CreateTestText();
            var parsed = Text.Parse(text.ToString());

            Assert.Equal(text.ToString(), parsed.ToString());
        }
    }
}
