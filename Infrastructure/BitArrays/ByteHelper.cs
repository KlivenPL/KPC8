using System;
using System.Collections;
using System.Collections.Generic;

namespace Infrastructure.BitArrays {
    public static class ByteHelper {

        public static T[] ToArray<T>(this byte[] bytes, int sizeofT, int skip, int take) {
            T[] tArr = new T[take / sizeofT];
            Buffer.BlockCopy(bytes, skip * sizeofT, tArr, 0, take);

            return tArr;
        }

        public static T[] ToArray<T>(this byte[] bytes, int sizeofT) {
            return bytes.ToArray<T>(sizeofT, 0, bytes.Length);
        }

        public static short[] ToShortArray(this byte[] bytes, int skip, int take) {
            return bytes.ToArray<short>(sizeof(short), skip, take);
        }

        public static short[] ToShortArray(this byte[] bytes) {
            return bytes.ToShortArray(0, bytes.Length);
        }

        public static byte[] DeepCopy(this byte[] bytes, int skip, int take) {
            return bytes.ToArray<byte>(sizeof(byte), skip, take);
        }

        public static byte[] DeepCopy(this byte[] bytes) {
            return bytes.DeepCopy(0, bytes.Length);
        }

        public static BitArray ToBitArray(this byte[] bytes) {
            return new BitArray(bytes);
        }

        public static BitArray ToBitArray(this byte @byte) {
            return new BitArray(@byte);
        }

        public static BitArray ToBitArrayLittleEndian(this byte @byte) {
            var ba = new BitArray(@byte);
            ba.Reverse();
            return ba;
        }

        public static BitArray[] To4BitArrays(this byte[] bytes) {
            var ba = new BitArray(bytes);

            List<BitArray> bitArrays = new List<BitArray>();
            for (int i = 0; i < ba.Length; i += 4) {
                bitArrays.Add(new BitArray(new[] { ba.Get(i), ba.Get(i + 1), ba.Get(i + 2), ba.Get(i + 3) }));
            }

            return bitArrays.ToArray();
        }

        public static BitArray[] To7BitArrays(this byte[] bytes) {
            var ba = new BitArray(bytes);

            var mod = ba.Length % 7;

            List<BitArray> bitArrays = new List<BitArray>();
            for (int i = 0; i < ba.Length; i += 7) {

                if (i + 7 > ba.Length) {
                    bool[] bools = new bool[7];

                    for (int j = 0; j < mod; j++) {
                        bools[j] = ba.Get(i + j);
                    }

                    bitArrays.Add(new BitArray(bools));
                } else {
                    bitArrays.Add(new BitArray(new[] { ba.Get(i), ba.Get(i + 1), ba.Get(i + 2), ba.Get(i + 3), ba.Get(i + 4), ba.Get(i + 5), ba.Get(i + 6) }));
                }
            }

            return bitArrays.ToArray();
        }
    }
}
