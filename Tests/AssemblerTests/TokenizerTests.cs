﻿using Assembler;
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
        [InlineData("add $t1 $t2 $t3", TokenClass.Identifier, TokenClass.Register, TokenClass.Register, TokenClass.Register)]
        [InlineData("add // comment, \r\n $t1, \r\n //ignore ignore \n\t\t   $t2 ,\n \t //ignore me $t4 \r\n\t$t3",
            TokenClass.Identifier, TokenClass.Register, TokenClass.Register, TokenClass.Register)]
        [InlineData(
            ".setaddress *region :label $t1 'c' \"string\" set test -1 2137 @const",
            TokenClass.Command, TokenClass.Region, TokenClass.Label, TokenClass.Register,
            TokenClass.Char, TokenClass.String, TokenClass.Identifier, TokenClass.Identifier, TokenClass.Number, TokenClass.Number, TokenClass.Identifier
        )]
        public void Tokenize_ValidInput_Success(string input, params TokenClass[] expectedTokenClasses) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var reader = CreateReader(ms);

            var tokens = new Tokenizer().Tokenize(reader).ToList();
            Assert.Equal(tokens.Select(t => t.Class), expectedTokenClasses);
        }

        [Theory]
        [InlineData("#what")]
        [InlineData("-what")]
        [InlineData("add \r\n %t1 \r\n \n\t\t   $t2 \n \t //ignore me $t4 \r\n\t$t3")]
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
