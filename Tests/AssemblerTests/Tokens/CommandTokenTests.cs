using Assembler.Commands;
using Assembler.Readers;
using Assembler.Tokens;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class CommandTokenTests {

        [Theory]
        [InlineData(".Address", CommandType.Address)]
        [InlineData(".ascii anyyy", CommandType.Ascii)]
        [InlineData(".ASCIIz", CommandType.Asciiz)]
        public void TryAccept_InputAccepted(string input, CommandType expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new CommandToken();
            var result = token.TryAccept(reader);

            Assert.True(result);
            Assert.Equal(expectedValue, token.Value);
        }

        [Theory]
        [InlineData(".none")]
        [InlineData(". Address")]
        [InlineData("Address")]
        [InlineData("*address")]
        public void TryAccept_InputNotAccepted(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new CommandToken();
            var result = token.TryAccept(reader);

            Assert.False(result);
            Assert.Equal(CommandType.None, token.Value);
        }

        private static CodeReader CreateReader(MemoryStream ms) {
            var reader = new CodeReader(ms);
            reader.Read();
            return reader;
        }
    }
}
