using Infrastructure.BitArrays;
using System.Collections;

namespace KPC8.RomProgrammers.Microcode {
    public static class McInstructionTypeExtensions {
        public static BitArray To16BitArray(this McInstructionType instruction) {
            return BitArrayHelper.FromUShortLE((ushort)instruction);
        }
    }
}
