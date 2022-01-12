using Infrastructure.BitArrays;
using System;
using System.Collections;

namespace KPC8.ProgRegs {
    public static class RegsExtensions {
        public static int GetIndex(this Regs reg) {
            if (reg == Regs.None) {
                throw new Exception("\"None\" is not a register.");
            }
            return (int)Math.Log2((ushort)reg);
        }

        public static BitArray GetEncodedAddress(this Regs reg) {
            byte index = (byte)reg.GetIndex();
            var encoded = BitArrayHelper.FromByteLE(index);
            return encoded.Skip(4);
        }

        public static BitArray GetDecodedAddress(this Regs reg) {
            return BitArrayHelper.FromUShort((ushort)reg);
        }
    }
}
