using Impostor.Api.Innersloth;
using Xunit;

namespace Impostor.Tests
{
    public class GameCodeTests
    {
        [Fact]
        public void CodeV1()
        {
            const string code = "ABCD";
            const int codeInt = 0x44434241;

            Assert.Equal(code, GameCodeParser.IntToGameName(codeInt));
            Assert.Equal(codeInt, GameCodeParser.GameNameToInt(code));
        }

        [Fact]
        public void CodeV2()
        {
            const string code = "ABCDEF";
            const int codeInt = -1943683525;

            Assert.Equal(code, GameCodeParser.IntToGameName(codeInt));
            Assert.Equal(codeInt, GameCodeParser.GameNameToInt(code));
        }
    }
}
