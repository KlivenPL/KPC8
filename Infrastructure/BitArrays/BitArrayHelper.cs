using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Infrastructure.BitArrays {
    public static class BitArrayHelper {

        public static BitArray MergeWith(this BitArray bitArray, BitArray otherBitArray) {
            byte[] mergedBytes;

            using (MemoryStream ms = new MemoryStream()) {
                bitArray.WriteBytesToMemStream(ms);
                otherBitArray.WriteBytesToMemStream(ms);
                mergedBytes = ms.ToArray();
            }
            return new BitArray(mergedBytes);
        }

        public static byte[] ToBytes(this BitArray bitArray) {
            var arrayLength = bitArray.Count / 8;

            byte[] array = new byte[arrayLength];
            bitArray.CopyTo(array, 0);

            return array;
        }

        public static void WriteBytesToMemStream(this BitArray bitArray, MemoryStream ms) {
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(bitArray.ToBytes());
        }

        public static bool EqualTo(this BitArray bitArray, BitArray other) {
            if (bitArray.Count != other.Count) {
                return false;
            }

            for (int i = 0; i < bitArray.Count; i++) {
                if (bitArray[i] != other[i]) {
                    return false;
                }
            }

            return true;
        }

        public static BitArray FromString(string str) {
            str = str.Replace(" ", "");

            if (str.Any(c => c != '0' && c != '1')) {
                throw new Exception("This string does not represent valid bit array");
            }

            return new BitArray(str.Select(c => c == '1').ToArray());
        }

        public static string ToBitString(this BitArray bitArray) {
            if (bitArray == null)
                return null;
            StringBuilder sb = new StringBuilder(bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++) {
                sb.Append(bitArray[i] ? "1" : "0");
            }

            return sb.ToString();
        }

        public static string ToBitStringWithDecAndHexLittleEndian(this BitArray bitArray) {
            if (bitArray == null)
                return null;
            StringBuilder sb = new StringBuilder(bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++) {
                sb.Append(bitArray[i] ? "1" : "0");
            }

            var @int = bitArray.ToIntLittleEndian();
            sb.Append($" (0x{BitConverter.ToString(BitConverter.GetBytes(@int).Reverse().ToArray())}) ({@int})");

            return sb.ToString();
        }

        public static double CompareTo(this BitArray bitArray, BitArray other) {
            if (bitArray == null || other == null)
                return 0;

            if (bitArray.Count != other.Count) {
                return 0;
            }

            int sameBits = 0;

            for (int i = 0; i < bitArray.Count; i++) {
                if (bitArray[i] == other[i]) {
                    sameBits++;
                }
            }

            return (double)sameBits / bitArray.Length;
        }

        public static BitArray Skip(this BitArray bitArray, int skip) {
            return FromString(bitArray.ToBitString().Substring(skip));
        }

        public static BitArray SkipLast(this BitArray bitArray, int skip) {
            return FromString(bitArray.ToBitString().Substring(0, bitArray.Count - skip));
        }

        public static byte ToByte(this BitArray bitArray) {
            if (bitArray.Length > 8)
                throw new Exception("Length must not be greater than 8");

            byte[] array = new byte[1];
            bitArray.CopyTo(array, 0);
            return array[0];
        }

        public static byte ToByteLittleEndian(this BitArray bitArray) {
            if (bitArray.Length > 8)
                throw new Exception("Length must not be greater than 8");

            var copy = new BitArray(bitArray);
            copy.Reverse();

            byte[] array = new byte[1];
            copy.CopyTo(array, 0);
            return array[0];
        }

        public static int ToIntLittleEndian(this BitArray bitArray) {
            if (bitArray.Length > 32)
                throw new Exception("Length must not be greater than 32");

            var copy = new BitArray(bitArray);
            copy.Reverse();

            byte[] array = new byte[4];
            copy.CopyTo(array, 0);
            return BitConverter.ToInt32(array);
        }

        public static void Reverse(this BitArray array) {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++) {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }
    }
}
