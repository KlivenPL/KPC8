using Assembler.Readers;
using Assembler.Tokens;
using KPC8.RomProgrammers.Microcode;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class OperationTokenTests {

        [Theory]
        [InlineData("nop", McInstructionType.Nop)]
        [InlineData("ADD", McInstructionType.Add)]
        [InlineData("aDdi", McInstructionType.AddI)]
        public void TryAccept_InputAccepted(string input, McInstructionType expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new OperationToken();
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

            var token = new OperationToken();
            var result = token.TryAccept(reader);

            Assert.False(result);
            Assert.Equal(McInstructionType.Nop, token.Value);
        }

        private static CodeReader CreateReader(MemoryStream ms) {
            var reader = new CodeReader(ms);
            reader.Read();
            return reader;
        }
    }
}
