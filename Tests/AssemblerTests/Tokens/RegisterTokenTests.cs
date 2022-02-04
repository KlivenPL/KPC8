using Assembler.Readers;
using Assembler.Tokens;
using KPC8.ProgRegs;
using System.IO;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Tokens {
    public class RegisterTokenTests {

        [Theory]
        [InlineData("$t1", Regs.T1)]
        [InlineData("$t1 $t2", Regs.T1)]
        [InlineData("$t1, $t2", Regs.T1)]
        [InlineData("$zErO", Regs.Zero)]
        public void TryAccept_InputAccepted(string input, Regs expectedValue) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new RegisterToken();
            var result = token.TryAccept(reader);

            Assert.True(result);
            Assert.Equal(expectedValue, token.Value);
        }

        [Theory]
        [InlineData("$ts1")]
        [InlineData("$ t1 $t2")]
        [InlineData("zErO")]
        [InlineData(".ra,$b")]
        public void TryAccept_InputNotAccepted(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var token = new RegisterToken();
            var result = token.TryAccept(reader);

            Assert.False(result);
            Assert.Equal(Regs.None, token.Value);
        }

        private static CodeReader CreateReader(MemoryStream ms) {
            var reader = new CodeReader(ms);
            reader.Read();
            return reader;
        }
    }
}
