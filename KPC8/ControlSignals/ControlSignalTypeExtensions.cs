using Infrastructure.BitArrays;
using System.Collections;

namespace KPC8.ControlSignals {
    public static class ControlSignalTypeExtensions {
        public static BitArray ToBitArray(this ControlSignalType controlSignal) {
            return BitArrayHelper.FromUIntLE((uint)controlSignal);
        }
    }
}
