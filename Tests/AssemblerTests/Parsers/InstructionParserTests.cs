using Assembler;
using Assembler._Infrastructure;
using Assembler.Contexts;
using Assembler.Contexts.Labels;
using Assembler.Encoders;
using Assembler.Parsers;
using Assembler.Readers;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.IO;
using System.Linq;
using System.Text;
using Tests._Infrastructure;
using Xunit;

namespace Tests.AssemblerTests.Parsers {
    public class InstructionParserTests {

        private readonly InstructionEncoder instructionEncoder = new InstructionEncoder();

        [Theory]
        [InlineData("nop", McInstructionType.Nop, Regs.Zero, Regs.Zero, Regs.Zero)]
        [InlineData("lbromo $t2, $t3, $ass", McInstructionType.Lbromo, Regs.T2, Regs.T3, Regs.Ass)]
        [InlineData("sbramo $t2, $t1, $ass", McInstructionType.Sbramo, Regs.T2, Regs.T1, Regs.Ass)]
        [InlineData("add $t2, $t3, $ass", McInstructionType.Add, Regs.T2, Regs.T3, Regs.Ass)]
        [InlineData("XOR $t2, $t3, $ass", McInstructionType.Xor, Regs.T2, Regs.T3, Regs.Ass)]
        [InlineData("set $t2 $t3", McInstructionType.Set, Regs.Zero, Regs.T2, Regs.T3)]
        [InlineData("jr $t2", McInstructionType.Jr, Regs.Zero, Regs.Zero, Regs.T2)]
        [InlineData("jwz $t1 $t3", McInstructionType.Jwz, Regs.Zero, Regs.T1, Regs.T3)]
        [InlineData("lbromo $t2 $t3, $ass $ra hehe", McInstructionType.Lbromo, Regs.T2, Regs.T3, Regs.Ass)]
        [InlineData("nop $t1 XD", McInstructionType.Nop, Regs.Zero, Regs.Zero, Regs.Zero)]

        public void ParseRegisterInstruction_SuccessfullyParsed(string input, McInstructionType expectedInstruction, Regs expectedRegDest, Regs expectedRegA, Regs expectedRegB) {
            var reader = CreateReader(input);
            var parser = CreateParser();

            parser.Parse(reader, out var instructionHigh, out var instructionLow);
            instructionEncoder.Encode(expectedInstruction, expectedRegDest, expectedRegA, expectedRegB, out var expectedInstructionHigh, out var expectedInstructionLow);

            BitAssert.Equality(expectedInstructionHigh, instructionHigh);
            BitAssert.Equality(expectedInstructionLow, instructionLow);
            Assert.Equal(8, instructionLow.Length);
            Assert.Equal(8, instructionHigh.Length);
        }

        [Theory]
        [InlineData("addi $t2, 218", McInstructionType.AddI, Regs.T2, 218)]
        [InlineData("irrex 218", McInstructionType.Irrex, Regs.T1, 218)]
        [InlineData("irren", McInstructionType.Irren, Regs.T1, 0)]
        [InlineData("seti $ass 'a'", McInstructionType.SetI, Regs.Ass, 'a')]

        public void ParseImmediateInstruction_SuccessfullyParsed(string input, McInstructionType expectedInstruction, Regs expectedRegDest, byte expectedImmediate) {
            var reader = CreateReader(input);
            var parser = CreateParser();

            parser.Parse(reader, out var instructionHigh, out var instructionLow);
            instructionEncoder.Encode(expectedInstruction, expectedRegDest, expectedImmediate, out var expectedInstructionHigh, out var expectedInstructionLow);

            BitAssert.Equality(expectedInstructionHigh, instructionHigh);
            BitAssert.Equality(expectedInstructionLow, instructionLow);
            Assert.Equal(8, instructionLow.Length);
            Assert.Equal(8, instructionHigh.Length);
        }

        [Theory]
        [InlineData("nopa")]
        [InlineData("lbromo $t2, $t3")]
        [InlineData("add $t2, 21, 37")]
        [InlineData("XOR :t2, $t3, $ass")]
        [InlineData("sbramo $t2, $ass")]

        public void ParseRegisterInstruction_ParserExceptionThrown(string input) {
            var reader = CreateReader(input);
            var parser = CreateParser();

            Assert.Throws<ParserException>(() => {
                parser.Parse(reader, out var instructionHigh, out var instructionLow);
            });
        }

        [Theory]
        [InlineData("adddi $t2, 218")]
        [InlineData("addi $t2 $t1")]
        [InlineData("addi $t2")]
        [InlineData("jpcaddi $t1 0x21FF")]
        [InlineData("irrex")]
        [InlineData("seti $ass \"a\"")]

        public void ParseImmediateInstruction_ParserExceptionThrown(string input) {
            var reader = CreateReader(input);
            var parser = CreateParser();

            Assert.Throws<ParserException>(() => {
                parser.Parse(reader, out var instructionHigh, out var instructionLow);
            });
        }

        private static TokenReader CreateReader(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var codeReader = new CodeReader(ms);
            var tokens = new Tokenizer().Tokenize(codeReader).ToList();

            var tokenReader = new TokenReader(tokens);
            tokenReader.Read();
            return tokenReader;
        }

        private static InstructionParser CreateParser() {
            return new InstructionParser(new InstructionsContext(), new InstructionEncoder(), new LabelsContext(new RegionParser()));
        }
    }
}
