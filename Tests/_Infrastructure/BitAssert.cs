using _Infrastructure.BitArrays;
using Components.Signals;
using Infrastructure.BitArrays;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests._Infrastructure {
    public static class BitAssert {
        public static void Equality(BitArray bitArray, ICollection<Signal> signals, string additionalMessage) {
            if (bitArray.Length != signals.Count) {
                Assert.True(false, $"{additionalMessage}Expected BitArray length ({bitArray.Length}) != to \r\nActual Signals count ({signals.Count()})\r\n");
            }

            for (int i = 0; i < bitArray.Length; i++) {
                if (signals.ElementAt(i) != bitArray[i])
                    Assert.True(false, $"{additionalMessage}Expected BitArray:\t\t {bitArray.ToPrettyBitString()}\r\nActual Signals:\t\t\t {signals.ToBitArray().ToPrettyBitString()}\r\n");
            }
        }

        public static void Equality(BitArray bitArray, ICollection<Signal> signals) {
            Equality(bitArray, signals, null);
        }

        public static void Equality(BitArray bitArray1, BitArray bitArray2, string additionalMessage) {
            if (bitArray1.Length != bitArray2.Length) {
                Assert.True(false, $"{additionalMessage}Expected BitArray length ({bitArray1.Length}) != to \r\nActual BitArray lenght ({bitArray2.Length})\r\n");
            }

            for (int i = 0; i < bitArray1.Length; i++) {
                if (bitArray1[i] != bitArray2[i])
                    Assert.True(false, $"{additionalMessage}Expected BitArray:\t\t {bitArray1.ToPrettyBitString()}\r\nActual BitArray:\t\t {bitArray2.ToPrettyBitString()}\r\n");
            }
        }

        public static void Equality(BitArray bitArray1, BitArray bitArray2) {
            Equality(bitArray1, bitArray2, null);
        }

        public static void Equality(byte[] expected, byte[] actual, string additionalMessage = "") {
            // First, ensure the arrays have the same length.
            if (expected.Length != actual.Length) {
                throw new Xunit.Sdk.XunitException(
                    $"{additionalMessage}Byte arrays have different lengths: expected {expected.Length}, but got {actual.Length}");
            }

            var differences = new List<string>();

            // Compare each byte.
            for (int i = 0; i < expected.Length; i++) {
                if (expected[i] != actual[i]) {
                    differences.Add(
                        $"At position 0x{i:X4}: expected 0x{expected[i]:X2}, but got 0x{actual[i]:X2}");
                }
            }

            // If any differences were found, throw an exception with all differences.
            if (differences.Count > 0) {
                throw new Xunit.Sdk.XunitException($"{additionalMessage}Byte arrays differs.{Environment.NewLine}{string.Join(Environment.NewLine, differences)}");
            }
        }
    }
}
