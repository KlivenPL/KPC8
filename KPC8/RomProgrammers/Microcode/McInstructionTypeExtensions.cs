using Infrastructure.BitArrays;
using System.Collections;

namespace KPC8.RomProgrammers.Microcode {
    public static class McInstructionTypeExtensions {
        public static BitArray Get6BitsOPCode(this McInstructionType instruction) {
            return BitArrayHelper.FromByteLE((byte)instruction).Skip(2);
        }
    }
}
