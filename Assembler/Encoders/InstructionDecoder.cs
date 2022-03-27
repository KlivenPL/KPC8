using Infrastructure.BitArrays;
using KPC8.ProgRegs;
using KPC8.RomProgrammers.Microcode;
using System.Collections;
using System.Linq;

namespace Assembler.Encoders {
    class InstructionDecoder {
        public void Decode(BitArray instructionHigh, BitArray instructionLow, out McInstructionType instructionType, out Regs regDest, out Regs regA, out Regs regB) {
            instructionType = (McInstructionType)BitArrayHelper.ToByteLE(new BitArray(2).MergeWith(instructionHigh.Take(6)));
            regDest = RegsExtensions.GetFromEncodedAddress(BitArrayHelper.FromString("01").MergeWith(instructionHigh.Skip(6)));
            regA = RegsExtensions.GetFromEncodedAddress(instructionLow.Take(4));
            regB = RegsExtensions.GetFromEncodedAddress(instructionLow.Skip(4));
        }

        public void Decode(BitArray instructionHigh, BitArray instructionLow, out McInstructionType instructionType, out Regs regDest, out byte imm) {
            instructionType = (McInstructionType)BitArrayHelper.ToByteLE(new BitArray(2).MergeWith(instructionHigh.Take(6)));
            regDest = RegsExtensions.GetFromEncodedAddress(BitArrayHelper.FromString("01").MergeWith(instructionHigh.Skip(6)));
            imm = BitArrayHelper.ToByteLE(instructionLow);
        }
    }
}
