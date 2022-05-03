using Assembler;
using Assembler.Encoders;
using Assembler.Readers;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Tests.AssemblerTests.Parsers.MainParser {
    public class PseudoinstructionParsingTests {

        private readonly Parser parser = new Parser();
        private readonly InstructionDecoder instructionDecoder = new InstructionDecoder();

        [Fact]
        public void ParseAddwIPseudoinstruction() {
            var input =
                @"
                addwi $t1 2137
                addwi $t2 0xFF01
                ";

            var rom = parser.Parse(CreateReader(input));

            instructionDecoder.Decode(rom[0], rom[1], out var instrType1, out var regDest1, out var imm1);
            instructionDecoder.Decode(rom[2], rom[3], out var instrType2, out var regDest2, out var imm2);
            instructionDecoder.Decode(rom[4], rom[5], out var instrType3, out var regDest3, out var regA3, out var regB3);

            Assert.Equal(McInstructionType.SetI, instrType1);
            Assert.Equal(Regs.Ass, regDest1);
            Assert.Equal(0x59, imm1);

            Assert.Equal(McInstructionType.SethI, instrType2);
            Assert.Equal(Regs.Ass, regDest2);
            Assert.Equal(0x8, imm2);

            Assert.Equal(McInstructionType.Addw, instrType3);
            Assert.Equal(Regs.T1, regDest3);
            Assert.Equal(Regs.T1, regA3);
            Assert.Equal(Regs.Ass, regB3);

            instructionDecoder.Decode(rom[6], rom[7], out var instrType4, out var regDest4, out var imm4);
            instructionDecoder.Decode(rom[8], rom[9], out var instrType5, out var regDest5, out var imm5);
            instructionDecoder.Decode(rom[10], rom[11], out var instrType6, out var regDest6, out var regA6, out var regB6);

            Assert.Equal(McInstructionType.SetI, instrType4);
            Assert.Equal(Regs.Ass, regDest4);
            Assert.Equal(0x01, imm4);

            Assert.Equal(McInstructionType.SethI, instrType5);
            Assert.Equal(Regs.Ass, regDest5);
            Assert.Equal(0xFF, imm5);

            Assert.Equal(McInstructionType.Addw, instrType6);
            Assert.Equal(Regs.T2, regDest6);
            Assert.Equal(Regs.T2, regA6);
            Assert.Equal(Regs.Ass, regB6);
        }

        [Fact]
        public void ParseAddJlPseudoinstruction() {
            var input =
                @"
                jl jumpLabel
                addwi $t1 2137
                :jumpLabel
                addwi $t2 0xFF01
                jl jumpLabel
                ";

            var rom = parser.Parse(CreateReader(input));

            instructionDecoder.Decode(rom[0], rom[1], out var instrType1, out var regDest1, out var imm1);
            instructionDecoder.Decode(rom[2], rom[3], out var instrType2, out var regDest2, out var imm2);
            instructionDecoder.Decode(rom[4], rom[5], out var instrType3, out var regDest3, out var regA3, out var regB3);

            Assert.Equal(McInstructionType.SetI, instrType1);
            Assert.Equal(Regs.Ass, regDest1);
            Assert.Equal(12, imm1);

            Assert.Equal(McInstructionType.SethI, instrType2);
            Assert.Equal(Regs.Ass, regDest2);
            Assert.Equal(0, imm2);

            Assert.Equal(McInstructionType.Jr, instrType3);
            Assert.Equal(Regs.T1, regDest3);
            Assert.Equal(Regs.Zero, regA3);
            Assert.Equal(Regs.Ass, regB3);

            instructionDecoder.Decode(rom[6], rom[7], out var instrType4, out var regDest4, out var imm4);
            instructionDecoder.Decode(rom[8], rom[9], out var instrType5, out var regDest5, out var imm5);
            instructionDecoder.Decode(rom[10], rom[11], out var instrType6, out var regDest6, out var regA6, out var regB6);

            Assert.Equal(McInstructionType.SetI, instrType4);
            Assert.Equal(Regs.Ass, regDest4);
            Assert.Equal(0x59, imm4);

            Assert.Equal(McInstructionType.SethI, instrType5);
            Assert.Equal(Regs.Ass, regDest5);
            Assert.Equal(0x8, imm5);

            Assert.Equal(McInstructionType.Addw, instrType6);
            Assert.Equal(Regs.T1, regDest6);
            Assert.Equal(Regs.T1, regA6);
            Assert.Equal(Regs.Ass, regB6);

            instructionDecoder.Decode(rom[12], rom[13], out var instrType7, out var regDest7, out var imm7);
            instructionDecoder.Decode(rom[14], rom[15], out var instrType8, out var regDest8, out var imm8);
            instructionDecoder.Decode(rom[16], rom[17], out var instrType9, out var regDest9, out var regA9, out var regB9);

            Assert.Equal(McInstructionType.SetI, instrType7);
            Assert.Equal(Regs.Ass, regDest7);
            Assert.Equal(0x01, imm7);

            Assert.Equal(McInstructionType.SethI, instrType8);
            Assert.Equal(Regs.Ass, regDest8);
            Assert.Equal(0xFF, imm8);

            Assert.Equal(McInstructionType.Addw, instrType9);
            Assert.Equal(Regs.T2, regDest9);
            Assert.Equal(Regs.T2, regA9);
            Assert.Equal(Regs.Ass, regB9);

            instructionDecoder.Decode(rom[18], rom[19], out var instrType10, out var regDest10, out var imm10);
            instructionDecoder.Decode(rom[20], rom[21], out var instrType11, out var regDest11, out var imm11);
            instructionDecoder.Decode(rom[22], rom[23], out var instrType13, out var regDest13, out var regA13, out var regB13);

            Assert.Equal(McInstructionType.SetI, instrType10);
            Assert.Equal(Regs.Ass, regDest10);
            Assert.Equal(12, imm10);

            Assert.Equal(McInstructionType.SethI, instrType11);
            Assert.Equal(Regs.Ass, regDest11);
            Assert.Equal(0, imm11);

            Assert.Equal(McInstructionType.Jr, instrType13);
            Assert.Equal(Regs.T1, regDest13);
            Assert.Equal(Regs.Zero, regA13);
            Assert.Equal(Regs.Ass, regB13);
        }

        private static TokenReader CreateReader(string input) {
            using var ms = new MemoryStream(Encoding.ASCII.GetBytes(input));
            using var codeReader = new CodeReader(ms);
            var tokens = new Tokenizer().Tokenize(codeReader).ToList();

            return new TokenReader(tokens);
        }
    }
}
