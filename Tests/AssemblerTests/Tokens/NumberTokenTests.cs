using Assembler.Readers;
using Assembler.Tokens;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class NumberTokenTests {

        [Theory]
        // Standard numeric literal tests
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

        // Expression tests in { } using integer arithmetic
        [InlineData("{123}", 123)]
        [InlineData("{0xF000 + 313}", 0xF139)]
        [InlineData("{0xf000 + 473}", 0xf1d9)]
        [InlineData("{ 123 }", 123)]
        [InlineData("{1+2}", 3)]
        [InlineData("{10-5}", 5)]
        [InlineData("{2*3}", 6)]
        [InlineData("{10/2}", 5)]
        [InlineData("{2+3*4}", 14)]       // multiplication before addition
        [InlineData("{1+2*3+4}", 11)]
        [InlineData("{(2+3)*4}", 20)]
        [InlineData("{2*(3+4)}", 14)]
        [InlineData("{(10-2)/(3+1)}", 2)]
        [InlineData("{1+2-3*4/2}", -3)]
        [InlineData("{-123}", -123)]
        [InlineData("{+7}", 7)]
        [InlineData("{7/2}", 3)]          // integer division truncates toward zero
        [InlineData("{1 + + 2}", 3)]
        [InlineData("{(1+(2*(3+(4-5))))}", 5)]
        // Complex nested expression using integer arithmetic:
        // ((2+0b10)-0xf*10) = (2+2) - 15*10 = 4 - 150 = -146, (21+37)=58, so -146/58 = -2
        [InlineData("{((2+0B10)-0xf*10)/(21+37)}", -2)]

        // Expression tests in { } using floating-point arithmetic (trailing 'f')
        // In these cases integer division would truncate, but using floating point the result is calculated more precisely.
        // Example: {1/3*5+1}f -> 1/3 = 0.3333... *5 = 1.6666... +1 = 2.6666... truncates to 2.
        [InlineData("{1/3*5+1}f", 2)]
        [InlineData("{2+2}f", 4)]
        [InlineData("{10/3}f", 3)]
        [InlineData("{(2+3)*4}f", 20)]
        [InlineData("{(10-2)/(3+1)}f", 2)]
        public void TryAccept_InputAccepted(string input, int expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new NumberToken();
            var result = token.TryAccept(reader);

            Assert.True(result);
            Assert.Equal((ushort)expectedValue, token.Value);
        }

        [Theory]
        // Negative tests for numeric literals and expressions (integer mode)
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

        // Negative tests for expressions in { } (integer mode)
        [InlineData("{-0x15f}")]
        [InlineData("{-0b101}")]
        [InlineData("{1+}")]
        [InlineData("{+}")]
        [InlineData("{(1+2}")]
        [InlineData("{1+2)}")]
        [InlineData("{1+0b2}")]      // invalid binary literal ('0b2' is not valid)
        [InlineData("{10/0}")]       // division by zero should cause failure
        [InlineData("{1*/2}")]
        [InlineData("{}")]           // empty expression not allowed
        [InlineData("{1+*2}")]

        // Negative tests for expressions in { } using floating-point mode (trailing 'f')
        [InlineData("{1/0}f")]       // division by zero
        [InlineData("{-0x15f}f")]     // negative hex not allowed even in float mode
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
