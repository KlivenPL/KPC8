using Infrastructure.BitArrays;
using System.Collections;

namespace KPC8.ControlSignals {
    public static class ControlSignalTypeExtensions {
        public static BitArray ToBitArray(this ControlSignalType controlSignal) {
            return BitArrayHelper.FromULongLE((ulong)controlSignal).Skip(24);
        }

        public static ControlSignalType FromBitArray(BitArray bitArray) {
            return (ControlSignalType)bitArray.GetUnsignedLongValueLE();
        }
    }
}
