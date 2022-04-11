using Infrastructure.BitArrays;
using System;
using System.Collections;
using System.Linq;

namespace Runner._Infrastructure {
    internal static class DebugValueFormatter {

        internal static string ToBinString(this BitArray ba) {
            return ba.ToPrettyBitString();
        }

        internal static string ToHexWordString(this BitArray ba) {
            return $"0x{ba.ToUShortLE:X4}";
        }

        internal static string ToHexTwoBytesString(this BitArray ba) {
            return $"0x{ba.Take(8).ToByteLE:X2}\t0x{ba.Skip(8).ToByteLE:X2}";
        }

        internal static string ToDecUnsignedWordString(this BitArray ba) {
            return ba.ToUShortLE().ToString();
        }

        internal static string ToDecUnsignedTwoBytesString(this BitArray ba) {
            return $"{ba.Take(8).ToByteLE}\t{ba.Skip(8).ToByteLE}";
        }

        internal static string ToDecSignedWordString(this BitArray ba) {
            return ba.ToShortLE().ToString();
        }

        internal static string ToDecSignedTwoBytesString(this BitArray ba) {
            return $"{ba.Take(8).ToSByteLE}\t{ba.Skip(8).ToSByteLE}";
        }

        internal static string ToFormattedDebugString(this BitArray ba, DebugValueFormat format) {
            return format switch {
                DebugValueFormat.Binary => ba.ToBinString(),
                DebugValueFormat.HexWord => ba.ToHexWordString(),
                DebugValueFormat.HexTwoBytes => ba.ToHexTwoBytesString(),
                DebugValueFormat.DecWordUnsigned => ba.ToDecUnsignedWordString(),
                DebugValueFormat.DecWordSigned => ba.ToDecSignedWordString(),
                DebugValueFormat.DecTwoBytesUnsigned => ba.ToDecUnsignedTwoBytesString(),
                DebugValueFormat.DecTwoBytesSigned => ba.ToDecSignedTwoBytesString(),
                _ => throw new NotImplementedException(),
            };
        }
    }

    public enum DebugValueFormat {
        Binary,
        HexWord,
        HexTwoBytes,
        DecWordUnsigned,
        DecWordSigned,
        DecTwoBytesUnsigned,
        DecTwoBytesSigned,
    }
}
