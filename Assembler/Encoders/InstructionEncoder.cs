using Infrastructure.BitArrays;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Linq;

namespace Assembler.Encoders {
    class InstructionEncoder {
        public void Encode(McInstructionType instructionType, Regs regDest, Regs regA, Regs regB, out BitArray instructionHigh, out BitArray instructionLow) {
            instructionHigh = BitArrayHelper.FromString($"{BitArrayHelper.FromUShortLE((ushort)instructionType).Skip(10).ToBitString()}{regDest.GetEncodedAddress().Skip(2).ToBitString()}");
            instructionLow = BitArrayHelper.FromString($"{regA.GetEncodedAddress().ToBitString()}{regB.GetEncodedAddress().ToBitString()}");
        }

        public void Encode(McInstructionType instructionType, Regs regDest, byte imm, out BitArray instructionHigh, out BitArray instructionLow) {
            instructionHigh = BitArrayHelper.FromString($"{BitArrayHelper.FromUShortLE((ushort)instructionType).Skip(10).ToBitString()}{regDest.GetEncodedAddress().Skip(2).ToBitString()}");
            instructionLow = BitArrayHelper.FromByteLE(imm);
        }
    }
}
