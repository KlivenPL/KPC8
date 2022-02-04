using Assembler.Readers;
using Assembler.Tokens;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class CharTokenTests {

        [Theory]
        [InlineData("'a'", 'a')]
        [InlineData("'1'", '1')]
        [InlineData("'1'123", '1')]
        [InlineData("'\\''", '\'')]
        [InlineData("'\\n'", '\n')]
        [InlineData("'\\t'", '\t')]
        [InlineData("'\\0'", '\0')]
        [InlineData("'\"'", '"')]
        public void TryAccept_InputAccepted(string input, char expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new CharToken();
            var result = token.TryAccept(reader);

            Assert.True(result);
            Assert.Equal(expectedValue, token.Value);
        }

        [Theory]
        [InlineData("''")]
        [InlineData("'")]
        [InlineData("'a")]
        [InlineData("'a ")]
        [InlineData("'a     ")]
        [InlineData("'a '")]
        [InlineData("a'")]
        [InlineData(" a'")]
        [InlineData("'a\\t'")]
        [InlineData("'a\t'")]
        [InlineData("$ra")]
        [InlineData("'\\r'")]
        [InlineData("'\\a'")]
        public void TryAccept_InputNotAccepted(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new CharToken();
            var result = token.TryAccept(reader);

            Assert.False(result);
            Assert.Equal(default, token.Value);
        }

        private static CodeReader CreateReader(MemoryStream ms) {
            var reader = new CodeReader(ms);
            reader.Read();
            return reader;
        }
    }
}
