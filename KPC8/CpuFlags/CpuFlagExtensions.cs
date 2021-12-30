using Infrastructure.BitArrays;
using System.Collections;

namespace KPC8.CpuFlags {
    public static class CpuFlagExtensions {
        public static BitArray To4BitArray(this CpuFlag flag) {
            return To8BitArray(flag).Skip(4);
        }

        public static BitArray To8BitArray(this CpuFlag flag) {
            return BitArrayHelper.FromByteLE((byte)flag);
        }
    }
}
