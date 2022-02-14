using Assembler.Readers;
using Assembler.Tokens;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class IdentifierTokenTests {

        [Theory]
        [InlineData("nop", "nop")]
        [InlineData("ADD", "ADD")]
        [InlineData("aDdi xd", "aDdi")]
        [InlineData("label_name-pig", "label_name")]
        public void TryAccept_InputAccepted(string input, string expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new IdentifierToken();
            var result = token.TryAccept(reader);

            Assert.True(result);
            Assert.Equal(expectedValue, token.Value);
        }

        [Theory]
        [InlineData(".nop")]
        [InlineData(". ADD")]
        [InlineData(" add")]
        [InlineData("*add")]
        public void TryAccept_InputNotAccepted(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new IdentifierToken();
            var result = token.TryAccept(reader);

            Assert.False(result);
            Assert.Null(token.Value);
        }

        private static CodeReader CreateReader(MemoryStream ms) {
            var reader = new CodeReader(ms);
            reader.Read();
            return reader;
        }
    }
}
