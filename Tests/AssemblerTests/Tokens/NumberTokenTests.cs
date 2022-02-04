using Assembler.Readers;
using Assembler.Tokens;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class NumberTokenTests {

        [Theory]
        [InlineData("0", 0)]
        [InlineData("013", 013)]
        [InlineData("128", 128)]
        [InlineData("2137", 2137)]
        [InlineData("21 37", 21)]
        [InlineData("21,37", 21)]
        [InlineData("32768", 32768)]
        [InlineData("65535", 65535)]
        [InlineData("-0", 0)]
        [InlineData("-128", -128)]
        [InlineData("-2137", -2137)]
        [InlineData("-32768", -32768)]
        [InlineData("0x0", 0x0)]
        [InlineData("0X0", 0x0)]
        [InlineData("0x15f", 0x15F)]
        [InlineData("0x00FF", 0x00FF)]
        [InlineData("0xFFFF", 0xFFFF)]
        [InlineData("0b0", 0b0)]
        [InlineData("0B0", 0b0)]
        [InlineData("0b101", 0b101)]
        [InlineData("0b11110111", 0b11110111)]
        [InlineData("0b10110111", 0b10110111)]
        [InlineData("0b1111111111111111", 0b1111111111111111)]
        public void TryAccept_InputAccepted(string input, int expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new NumberToken();
            var result = token.TryAccept(reader);

            Assert.True(result);
            Assert.Equal((ushort)expectedValue, token.Value);
        }

        [Theory]
        [InlineData("65536")]
        [InlineData("-32769")]
        [InlineData(",0")]
        [InlineData("0abc")]
        [InlineData("-1abc")]
        [InlineData("-abc")]
        [InlineData("--2137")]
        [InlineData("0xFFFFF")]
        [InlineData("0x")]
        [InlineData("0x01xF")]
        [InlineData("x01")]
        [InlineData("FF")]
        [InlineData("-0x15f")]
        [InlineData("0b11111111111111111")]
        [InlineData("0b10121")]
        [InlineData("0b01b0")]
        [InlineData("0b")]
        [InlineData("-0b101")]
        [InlineData("b01")]
        public void TryAccept_InputNotAccepted(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new NumberToken();
            var result = token.TryAccept(reader);

            Assert.False(result);
            Assert.Equal(0, token.Value);
        }

        private static CodeReader CreateReader(MemoryStream ms) {
            var reader = new CodeReader(ms);
            reader.Read();
            return reader;
        }
    }
}
