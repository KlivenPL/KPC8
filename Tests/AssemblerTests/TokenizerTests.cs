using Assembler;
using Assembler._Infrastructure;
using Assembler.Readers;
using Assembler.Tokens;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests {
    public class TokenizerTests {

        [Theory]
        [InlineData("add $t1 $t2 $t3", TokenClass.Operation, TokenClass.Register, TokenClass.Register, TokenClass.Register)]
        [InlineData("add, \r\n $t1, \r\n //ignore ignore \n\t\t   $t2 ,\n \t //ignore me $t4 \r\n\t$t3",
            TokenClass.Operation, TokenClass.Register, TokenClass.Register, TokenClass.Register)]
        [InlineData(
            ".address *region :label $t1 'c' \"string\" set -1 2137",
            TokenClass.Command, TokenClass.Region, TokenClass.Label, TokenClass.Register,
            TokenClass.Char, TokenClass.String, TokenClass.Operation, TokenClass.Number, TokenClass.Number
        )]
        public void Tokenize_ValidInput_Success(string input, params TokenClass[] expectedTokenClasses) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var tokens = new Tokenizer().Tokenize(reader).ToList();
            Assert.Equal(tokens.Select(t => t.Class), expectedTokenClasses);
        }

        [Theory]
        [InlineData("#what")]
        [InlineData("add \r\n %t1 \r\n \n\t\t   $t2 \n \t //ignore me $t4 \r\n\t$t3")]
        [InlineData("ad $t1")]
        public void Tokenize_InvalidInput_ErrorThrown(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            Assert.Throws<TokenizerException>(() => new Tokenizer().Tokenize(reader).ToList());
        }

        private static CodeReader CreateReader(MemoryStream ms) {
            var reader = new CodeReader(ms);
            return reader;
        }
    }
}
