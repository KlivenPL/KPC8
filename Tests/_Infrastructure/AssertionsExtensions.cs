using _Infrastructure.BitArrays;
using Components.Signals;
using Infrastructure.BitArrays;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests._Infrastructure {
    public static class BitAssert {
        public static void Equality(BitArray bitArray, ICollection<Signal> signals) {
            if (bitArray.Length != signals.Count) {
                Assert.True(false, $"BitArray length ({bitArray.Length}) != to Signals count ({signals.Count()})");
            }

            for (int i = 0; i < bitArray.Length; i++) {
                if (signals.ElementAt(i) != bitArray[i])
                    Assert.True(false, $"BitArray {bitArray.ToBitString()} differs to Signals {signals.ToBitArray().ToBitString()}");
            }
        }

        public static void Equality(BitArray bitArray1, BitArray bitArray2) {
            if (bitArray1.Length != bitArray2.Length) {
                Assert.True(false, $"BitArray1 length ({bitArray1.Length}) != to BitArray2 lenght ({bitArray2.Length})");
            }

            for (int i = 0; i < bitArray1.Length; i++) {
                if (bitArray1[i] != bitArray2[i])
                    Assert.True(false, $"BitArray1: {bitArray1.ToBitString()} differs to BitArray2: {bitArray2.ToBitString()}");
            }
        }
    }
}
