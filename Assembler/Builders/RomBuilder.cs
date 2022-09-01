using Infrastructure.BitArrays;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assembler.Builders {
    class RomBuilder {
        private const ushort MaxAddress = ushort.MaxValue;
        private readonly List<BitArray> compiled;
        private readonly bool[] reservedAddresses;
        private ushort nextAddress;
        public ushort NextAddress {
            get => nextAddress;
            set {
                overflow = false;
                nextAddress = value;
            }
        }

        private bool overflow = false;

        public RomBuilder() {
            compiled = new List<BitArray>();
            for (int i = 0; i <= MaxAddress; i++) {
                compiled.Add(null);
            }
            reservedAddresses = new bool[MaxAddress + 1];
        }

        public RomBuilder AddInstruction(BitArray instructionHigh, BitArray instructionLow, out ushort loAddress) {
            AddByte(instructionHigh);
            loAddress = NextAddress;
            AddByte(instructionLow);
            return this;
        }

        public RomBuilder AddPseudoinstruction(BitArray[] instructions, out ushort loAddress) {
            if (instructions.Length % 2 != 0) {
                throw new System.Exception("Pseudoinstructions must be of length 2N");
            }
            foreach (var instruction in instructions) {
                AddByte(instruction);
            }
            loAddress = (ushort)(NextAddress - 1);
            return this;
        }

        public RomBuilder AddShort(BitArray @short) {
            if (@short.Length != 16) {
                throw new System.Exception($"Value {@short.ToBitString()} has {@short.Length} instead of 16 bits");
            }

            AddByte(@short.Take(8));
            AddByte(@short.Skip(8));
            return this;
        }

        public RomBuilder AddByte(BitArray @byte) {
            if (overflow) {
                throw new System.Exception("Out of memory or tried to write outside the bounds (64k ROM is MAX)");
            }

            if (@byte.Length != 8) {
                throw new System.Exception($"Value {@byte.ToBitString()} has {@byte.Length} instead of 8 bits");
            }

            if (!IsAddressFree(NextAddress)) {
                throw new System.Exception($"Address {NextAddress} is occupied or reserved");
            }

            compiled[NextAddress++] = @byte;
            overflow = NextAddress == 0;

            return this;
        }

        public void Reserve(int size) {
            if (nextAddress + size > reservedAddresses.Length) {
                throw new System.Exception("Out of memory or tried to write outside the bounds (64k ROM is MAX)");
            }

            for (int i = 0; i < size; i++) {
                reservedAddresses[nextAddress + i] = true;
            }
        }

        public void Unreserve(int size) {
            for (int i = 0; i < size; i++) {
                reservedAddresses[nextAddress + i] = false;
            }
        }

        public bool IsAddressFree(ushort address) {
            return compiled[address] == null && reservedAddresses[address] == false;
        }

        public BitArray[] Build() {
            if (reservedAddresses?.Any(x => x) == true) {
                throw new System.Exception("There should be no reserved addresses by now.");
            }

            for (int i = 0; i < compiled.Count; i++) {
                if (compiled[i] == null) {
                    compiled[i] = new BitArray(8);
                }
            }

            return compiled.ToArray();
        }
    }
}
