using Components.Signals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace _Infrastructure.BitArrays {
    public static class SignalsBitArrayExtensions {
        public static BitArray ToBitArray(this IEnumerable<Signal> signals) {
            return new BitArray(signals.Select(s => (bool)s).ToArray());
        }

        public static IEnumerable<Signal> ToSignals(this BitArray bitArray) {
            foreach (bool bit in bitArray) {
                var sig = Signal.Factory.GetOrCreate(Guid.NewGuid().ToString());
                sig.Value = bit;
                yield return sig;
            }
        }

        public static void WriteBitArray(this IEnumerable<Signal> signals, BitArray bitArray) {
            for (int i = 0; i < signals.Count(); i++) {
                signals.ElementAt(i).Value = bitArray[i];
            }
        }
    }
}
