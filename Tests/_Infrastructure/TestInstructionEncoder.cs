using Infrastructure.BitArrays;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Linq;

namespace Tests._Infrastructure {
    public class TestInstructionEncoder {
        public void EncodeInstruction(McInstruction instruction, Regs regDest, Regs regA, Regs regB, out BitArray instructionHigh, out BitArray instructionLow) {
            var opCode = instruction.OpCode;

            instructionHigh = BitArrayHelper.FromString($"{opCode.ToBitString()}{regDest.GetEncodedAddress().Skip(2).ToBitString()}");
            instructionLow = BitArrayHelper.FromString($"{regA.GetEncodedAddress().ToBitString()}{regB.GetEncodedAddress().ToBitString()}");
        }

        public void EncodeInstruction(McInstruction instruction, Regs regDest, BitArray imm, out BitArray instructionHigh, out BitArray instructionLow) {
            if (imm.Length != 8) {
                throw new System.Exception("IMM value must be 8 bits long");
            }

            var opCode = instruction.OpCode;

            instructionHigh = BitArrayHelper.FromString($"{opCode.ToBitString()}{regDest.GetEncodedAddress().Skip(2).ToBitString()}");
            instructionLow = imm;
        }
    }
}
