using Assembler.Readers;
using Assembler.Tokens;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class StringTokenTests {

        [Theory]
        [InlineData("\"ala ma kota\"", "ala ma kota")]
        [InlineData("\"a\"abc", "a")]
        [InlineData("\"0\"", "0")]
        [InlineData("\"!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[]^_`abcdefghijklmnopqrstuvwxyz{|}~\"", "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[]^_`abcdefghijklmnopqrstuvwxyz{|}~")]
        [InlineData("\"abc\\n\\t\\\"\\0\\\\cba\"", "abc\n\t\"\0\\cba")]
        [InlineData("\"\"", "")]
        public void TryAccept_InputAccepted(string input, string expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new StringToken();
            var result = token.TryAccept(reader);

            Assert.True(result);
            Assert.Equal(expectedValue, token.Value);
            Assert.Equal(expectedValue.Length, token.Value.Length);
        }

        [Theory]
        [InlineData("$ts1")]
        [InlineData("$ t1 $t2")]
        [InlineData("zErO")]
        [InlineData(".ra,$b")]
        [InlineData("1234")]
        [InlineData("abc\"1234")]
        [InlineData("\"")]
        [InlineData("\" ")]
        [InlineData("\"    ")]
        [InlineData("\"a")]
        [InlineData("\"abc")]
        [InlineData("abc\"")]
        [InlineData("\"\\r\"")]
        [InlineData("\"\\a\"")]
        public void TryAccept_InputNotAccepted(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new StringToken();
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
