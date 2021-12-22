using _Infrastructure.Names;
using Components.Signals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public static Signal CreateSignalAndPlugin<T>(this T obj, string nameof, Expression<Func<T, SignalPort>> expression) {
            var signal = Signal.Factory.Create(NameOf<T>.Full(nameof, expression));
            var func = expression.Compile();
            func(obj).PlugIn(signal);
            return signal;
        }
    }
}
