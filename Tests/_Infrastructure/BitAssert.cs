using _Infrastructure.BitArrays;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests._Infrastructure {
    public static class BitAssert {
        public static void Equality(BitArray bitArray, ICollection<Signal> signals, string additionalMessage) {
            if (bitArray.Length != signals.Count) {
                Assert.True(false, $"{additionalMessage}BitArray length ({bitArray.Length}) != to Signals count ({signals.Count()})");
            }

            for (int i = 0; i < bitArray.Length; i++) {
                if (signals.ElementAt(i) != bitArray[i])
                    Assert.True(false, $"{additionalMessage}BitArray {bitArray.ToPrettyBitString()} differs to Signals {signals.ToBitArray().ToPrettyBitString()}");
            }
        }

        public static void Equality(BitArray bitArray, ICollection<Signal> signals) {
            Equality(bitArray, signals, null);
        }

        public static void Equality(BitArray bitArray1, BitArray bitArray2, string additionalMessage) {
            if (bitArray1.Length != bitArray2.Length) {
                Assert.True(false, $"{additionalMessage}BitArray1 length ({bitArray1.Length}) != to BitArray2 lenght ({bitArray2.Length})");
            }

            for (int i = 0; i < bitArray1.Length; i++) {
                if (bitArray1[i] != bitArray2[i])
                    Assert.True(false, $"{additionalMessage}BitArray1: {bitArray1.ToPrettyBitString()} differs to BitArray2: {bitArray2.ToPrettyBitString()}");
            }
        }

        public static void Equality(BitArray bitArray1, BitArray bitArray2) {
            Equality(bitArray1, bitArray2, null);
        }
    }
}
